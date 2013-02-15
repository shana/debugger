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
	[Export(typeof(IFactory))]
	public class VMFactory : IFactory
	{
		public void Initialize ()
		{
			Factory.Register (() => new VirtualMachine ());
		}
	}


	[Export (typeof (IVirtualMachine))]
	public class VirtualMachine : Wrapper, IVirtualMachine
	{
		private bool running = true;
		private bool exited = false;
		private readonly List<Exception> errors = new List<Exception> ();
		private readonly List<MDS.AppDomainMirror> appdomains = new List<MDS.AppDomainMirror> ();

		private MDS.VirtualMachine vm;
		private IEventRequest methodEntryRequest;
		private List<long> filteredAssemblies = new List<long> ();

		public event Action<IEvent> VMStateChanged;
		public event Action<IEvent> VMSuspended;
		public event Action<IEvent> AppDomainLoaded;
		public event Action<IEvent> AppDomainUnloaded;
		public event Action<IEvent> ThreadStarted;
		public event Action<IEvent> ThreadStopped;
		public event Action<IAssemblyEvent> AssemblyLoaded;
		public event Action<IAssemblyEvent> AssemblyUnloaded;
		public event Action<ITypeEvent> TypeLoaded;
		public event Action<IBreakpointEvent> BreakpointHit;
		public event Action<IEvent> Stepped;

		public IList<IAssemblyMirror> RootAssemblies
		{
			get { return vm.RootDomain.GetAssemblies ().Select (a => Cache.Lookup<SdbAssemblyMirror> (a) as IAssemblyMirror).ToList ().AsReadOnly (); }
		}

		public IList<IAssemblyMirror> Assemblies
		{
			get { return appdomains.SelectMany (a => a.GetAssemblies ().Select (x => Cache.Lookup<SdbAssemblyMirror> (x) as IAssemblyMirror)).ToList ().AsReadOnly (); }
		}

		public IList<IThreadMirror> Threads {
			get { return vm.GetThreads ().Select (x => Cache.Lookup<SdbThreadMirror> (x) as IThreadMirror).ToList ().AsReadOnly (); }
		}

		public Process Process
		{
			get { return vm.Process; }
		}

		public IEnumerable<Exception> Errors
		{
			get { return errors; }
		}

		[ImportingConstructor]
		public VirtualMachine ()
			: base (null)
		{
			Factory.Register (
				location => new SdbBreakpoint (vm.CreateBreakpointRequest (location.Unwrap<MDS.Location> ()), location),
				(thread, stepType) =>
				{
					var request = vm.CreateStepRequest(thread.Unwrap<MDS.ThreadMirror>());
					switch (stepType)
					{
						case StepType.Into: request.Depth = MDS.StepDepth.Into; break;
						case StepType.Over: request.Depth = MDS.StepDepth.Over; break;
						case StepType.Out: request.Depth = MDS.StepDepth.Out; break;
					}
					return Cache.Lookup<EventRequest>(request);
				},
				() => new EventRequest (vm.CreateMethodEntryRequest ()),
				() => new EventRequest (vm.CreateMethodExitRequest ()),
				(source, line) => new SdbLocation (source, line)
				);
		}

		public void Attach (int port)
		{
			LogProvider.Log ("Attempting connection at port {0}...", port);
			vm = MDS.VirtualMachineManager.Connect (new IPEndPoint (IPAddress.Loopback, port));
			vm.EnableEvents (
				MDS.EventType.AppDomainCreate,
				MDS.EventType.AppDomainUnload,
				MDS.EventType.VMStart,
				MDS.EventType.VMDeath,
				MDS.EventType.VMDisconnect,
				MDS.EventType.AssemblyLoad,
				MDS.EventType.AssemblyUnload
			);

			methodEntryRequest = new EventRequest (vm.CreateMethodEntryRequest ());
			QueueUserWorkItem (EventLoop);
		}


		public void Suspend ()
		{
			vm.Suspend ();
			if (VMSuspended != null)
				VMSuspended (null);
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

			//if (VMSuspended != null && policy != MDS.SuspendPolicy.None)
			//{
			//    switch (e.EventType)
			//    {
			//        case MDS.EventType.VMStart:
			//        case MDS.EventType.VMDeath:
			//        case MDS.EventType.VMDisconnect:
			//        case MDS.EventType.AppDomainCreate:
			//        case MDS.EventType.AppDomainUnload:
			//        case MDS.EventType.ThreadStart:
			//        case MDS.EventType.ThreadDeath:
			//        case MDS.EventType.AssemblyLoad:
			//        case MDS.EventType.AssemblyUnload:
			//        case MDS.EventType.TypeLoad:
			//            break;
			//        default:
			//            LogProvider.Log (e.EventType.ToString ());
			//            OnVMSuspended (new Event (e, State.Suspend));
			//            break;
			//    }
			//}

			switch (e.EventType)
			{
				case MDS.EventType.VMStart:
					if (VMStateChanged != null)
						VMStateChanged (new Event (e, State.Start));
					break;
				case MDS.EventType.VMDeath:
					if (VMStateChanged != null)
						VMStateChanged (new Event (e, State.Stop));
					ret = false;
					break;
				case MDS.EventType.VMDisconnect:
					if (VMStateChanged != null)
						VMStateChanged (new Event (e, State.Disconnect));
					ret = false;
					break;
				case MDS.EventType.AppDomainCreate:
					if (AppDomainLoaded != null)
						AppDomainLoaded (new Event (e));
					appdomains.Add (((MDS.AppDomainCreateEvent)e).Domain);
					break;
				case MDS.EventType.AppDomainUnload:
					if (AppDomainUnloaded != null)
						AppDomainUnloaded (new Event (e));
					if (appdomains.Contains (((MDS.AppDomainUnloadEvent)e).Domain))
						appdomains.Remove (((MDS.AppDomainUnloadEvent)e).Domain);
					break;
				case MDS.EventType.ThreadStart:
					if (ThreadStarted != null)
						ThreadStarted (new Event (e));
					break;
				case MDS.EventType.ThreadDeath:
					if (ThreadStopped != null)
						ThreadStopped (new Event (e));
					break;
				case MDS.EventType.AssemblyLoad:
					if (AssemblyLoaded != null) {
						var ev = new AssemblyEvent (e);
						AssemblyLoaded (ev);
						if (!ev.Cancel && !filteredAssemblies.Contains (((MDS.AssemblyLoadEvent)e).Assembly.Id))
						{
							var tr = vm.CreateTypeLoadRequest ();
							tr.AssemblyFilter = new MDS.AssemblyMirror [] {((MDS.AssemblyLoadEvent)e).Assembly};
							tr.Enable ();
							filteredAssemblies.Add (((MDS.AssemblyLoadEvent)e).Assembly.Id);
						}
					}
					break;
				case MDS.EventType.AssemblyUnload:
					if (AssemblyUnloaded != null)
						AssemblyUnloaded (new AssemblyEvent (e));
					break;
				case MDS.EventType.TypeLoad:
					if (TypeLoaded != null)
						TypeLoaded (new TypeEvent (e));
					break;
				case MDS.EventType.Breakpoint:
					if (BreakpointHit != null)
						BreakpointHit (new BreakpointEvent (e));
					break;
				case MDS.EventType.Step:
					if (Stepped != null)
						Stepped (new Event (e));
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

	}
}
