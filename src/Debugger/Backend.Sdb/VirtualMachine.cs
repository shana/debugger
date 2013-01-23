using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Linq;
using Debugger.Backend.Event;
using MDS = Mono.Debugger.Soft;
using CodeEditor.Composition;

namespace Debugger.Backend.Sdb
{
	[Export (typeof (IVirtualMachine))]
	public class VirtualMachine : Wrapper, IVirtualMachine
	{
		private bool running = true;
		private bool exited = false;
		private readonly List<Exception> errors = new List<Exception> ();
		private readonly List<MDS.AppDomainMirror> appdomains = new List<MDS.AppDomainMirror> ();

		private MDS.VirtualMachine vm;
		private IEventRequest methodEntryRequest;

		public event Action<IEvent> OnVM;
		public event Action<IEvent> OnVMSuspended;
		public event Action<IEvent> OnAppDomain;
		public event Action<IEvent> OnThread;
		public event Action<IAssemblyEvent> OnAssembly;
		public event Action<ITypeEvent> OnType;
		public event Action<IBreakpointEvent> OnBreakpoint;
		public event Action<IEvent> OnStep;

		[ImportingConstructor]
		public VirtualMachine ()
			: base (null)
		{
			Factory.Register (
				location => new SdbBreakpoint (vm.CreateBreakpointRequest (location.Unwrap<MDS.Location> ()), location),
				thread => Cache.Lookup<EventRequest> (vm.CreateStepRequest (thread.Unwrap<MDS.ThreadMirror> ())),
				() => new EventRequest (vm.CreateMethodEntryRequest ()),
				() => new EventRequest (vm.CreateMethodExitRequest ()),
				(source, line) => new SdbLocation (source, line)
				);
		}

		public void Attach (int port)
		{
			LogProvider.Log ("Attempting connection at port {0}...", port);
			this.vm = MDS.VirtualMachineManager.Connect (new IPEndPoint (IPAddress.Loopback, port));
			this.vm.EnableEvents (
				MDS.EventType.AppDomainCreate,
				MDS.EventType.AppDomainUnload,
				MDS.EventType.VMStart,
				MDS.EventType.VMDeath,
				MDS.EventType.VMDisconnect,
				MDS.EventType.AssemblyLoad,
				MDS.EventType.AssemblyUnload,
				MDS.EventType.TypeLoad
			);

			methodEntryRequest = new EventRequest (this.vm.CreateMethodEntryRequest ());
			QueueUserWorkItem (EventLoop);
		}

		public IEnumerable<IAssemblyMirror> RootAssemblies
		{
			get { return vm.RootDomain.GetAssemblies ().Select (a => Cache.Lookup<SdbAssemblyMirror> (a) as IAssemblyMirror); }
		}

		public IEnumerable<IAssemblyMirror> Assemblies
		{
			get { return appdomains.SelectMany (a => a.GetAssemblies ().Select (x => Cache.Lookup<SdbAssemblyMirror> (x) as IAssemblyMirror)); }
		}

		public Process Process
		{
			get { return vm.Process; }
		}

		public void Suspend ()
		{
			vm.Suspend ();
		}

		public void Resume ()
		{
			try
			{
				vm.Resume ();
			}
			catch (InvalidOperationException)
			{
				int a = 0;
				//there is some racy bug somewhere that sometimes makes the runtime complain that we are resuming while we were not suspended.
				//obviously if you dont resume, the other 95% of the cases, you hang because we were suspended.
			}
		}

		public void Detach ()
		{
			vm.Detach ();
		}

		public IEnumerable<Exception> Errors
		{
			get { return errors; }
		}

		private void QueueUserWorkItem (Action a)
		{
			ThreadPool.QueueUserWorkItem (_ => LogProvider.WithErrorLogging (a));
		}

		private void EventLoop ()
		{
			while (running)
			{
				var e = vm.GetNextEventSet ();
				if (e == null)
					return;
				foreach (var evt in e.Events) {
					running = HandleEvent (evt, e.SuspendPolicy);
					if (!running)
						break;
				}
			}
		}

		private bool HandleEvent (MDS.Event e, MDS.SuspendPolicy policy)
		{
			LogProvider.Log ("Received Event: " + e.GetType ());

			bool exit = false;
			lock (vm)
			{
				exit = exited;
			}
			if (exit)
				return false;

			bool ret = running;

			if (OnVMSuspended != null && policy != MDS.SuspendPolicy.None)
			{
				switch (e.EventType)
				{
					case MDS.EventType.VMStart:
					case MDS.EventType.VMDeath:
					case MDS.EventType.VMDisconnect:
					case MDS.EventType.AppDomainCreate:
					case MDS.EventType.AppDomainUnload:
					case MDS.EventType.ThreadStart:
					case MDS.EventType.ThreadDeath:
					case MDS.EventType.AssemblyLoad:
					case MDS.EventType.AssemblyUnload:
					case MDS.EventType.TypeLoad:
						break;
					default:
						LogProvider.Log (e.EventType.ToString ());
						OnVMSuspended (new Event (e, State.Suspend));
						break;
				}
			}

			switch (e.EventType)
			{
				case MDS.EventType.VMStart:
					if (OnVM != null)
						OnVM (new Event (e, State.Start));
					break;
				case MDS.EventType.VMDeath:
					if (OnVM != null)
						OnVM (new Event (e, State.Stop));
					ret = false;
					break;
				case MDS.EventType.VMDisconnect:
					if (OnVM != null)
						OnVM (new Event (e, State.Disconnect));
					ret = false;
					break;
				case MDS.EventType.AppDomainCreate:
					appdomains.Add (((MDS.AppDomainCreateEvent)e).Domain);
					break;
				case MDS.EventType.AppDomainUnload:
					if (appdomains.Contains (((MDS.AppDomainUnloadEvent)e).Domain))
						appdomains.Add (((MDS.AppDomainUnloadEvent)e).Domain);
					break;
				case MDS.EventType.ThreadStart:
					if (OnThread != null)
						OnThread (new Event (e, State.Start));
					break;
				case MDS.EventType.ThreadDeath:
					if (OnThread != null)
						OnThread (new Event (e, State.Stop));
					break;
				case MDS.EventType.AssemblyLoad:
					if (OnAssembly != null)
						OnAssembly (new AssemblyEvent (e, State.Load));
					break;
				case MDS.EventType.AssemblyUnload:
					if (OnAssembly != null)
						OnAssembly (new AssemblyEvent (e, State.Unload));
					break;
				case MDS.EventType.TypeLoad:
					if (OnType != null)
						OnType (new TypeEvent (e, State.Load));
					break;
				case MDS.EventType.Breakpoint:
					if (OnBreakpoint != null)
						OnBreakpoint (new BreakpointEvent (e, State.None));
					break;
				case MDS.EventType.Step:
					if (OnStep != null)
						OnStep (new Event (e, State.None));
					break;
				case MDS.EventType.MethodEntry:
					LogProvider.Log (((MDS.MethodEntryEvent)e).Method.FullName);
					break;
				default:
					LogProvider.Log ("Unknown event: " + e.GetType ());
					break;
			}
			if (policy != MDS.SuspendPolicy.None)
				Resume ();

			return ret;
		}

		public void Exit ()
		{
			lock (vm)
			{
				exited = true;
			}
			vm.Exit (0);
			vm.Detach ();
		}

		public void ResumeIfNeeded ()
		{
		}

		public IList<IThreadMirror> GetThreads ()
		{
			return vm.GetThreads ().Select (x => Cache.Lookup<SdbThreadMirror> (x) as IThreadMirror).ToList ();
		}

		public void ClearAllBreakpoints ()
		{
			vm.ClearAllBreakpoints ();
		}
	}
}
