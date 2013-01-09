using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MDS=Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	internal class VirtualMachine : Wrapper, IVirtualMachine
	{
		private bool _running = true;
		private bool _exited = false;
		private readonly List<Exception> _errors = new List<Exception> ();

		private readonly MDS.VirtualMachine _vm;
		private IEventRequest _methodEntryRequest;

		public event Action<IEvent> OnVMStart;
		public event Action<IEvent> OnVMDeath;
		public event Action<IAssemblyLoadEvent> OnAssemblyLoad;

		public event Action<ITypeLoadEvent> OnTypeLoad;
		public event Action<IEvent> OnVMDisconnect;
		public event Action<IEvent> OnThreadStart;
		public event Action<IEvent> OnBreakpoint;
		public event Action<IEvent> OnVMGotSuspended;

		public VirtualMachine (MDS.VirtualMachine vm) : base(vm)
		{
			_vm = vm;
			_vm.Suspend ();
			_vm.EnableEvents (
				MDS.EventType.AssemblyLoad,
				MDS.EventType.VMDeath,
				MDS.EventType.TypeLoad,
				MDS.EventType.VMStart
			);

			_methodEntryRequest = new EventRequest (_vm.CreateMethodEntryRequest ());
			QueueUserWorkItem (EventLoop);
		}

		public Process Process
		{
			get { return _vm.Process; }
		}

		public void Suspend ()
		{
			_vm.Suspend ();
		}

		public void Resume ()
		{
			try
			{
				_vm.Resume ();
			}
			catch (InvalidOperationException)
			{
				//there is some racy bug somewhere that sometimes makes the runtime complain that we are resuming while we were not suspended.
				//obviously if you dont resume, the other 95% of the cases, you hang because we were suspended.
			}
		}

		public IEnumerable<Exception> Errors {
			get { return _errors; }
		}

		private void QueueUserWorkItem (Action a)
		{
			ThreadPool.QueueUserWorkItem (_ => WithErrorLogging (a));
		}

		private void EventLoop ()
		{
			while (_running)
			{
				var e = _vm.GetNextEventSet ();
				if (e == null)
					return;
				foreach (var evt in e.Events)
					HandleEvent (evt, e.SuspendPolicy);
			}
		}

		private void HandleEvent (MDS.Event e, MDS.SuspendPolicy policy)
		{
			Console.WriteLine("Event: " + e.GetType());
			bool exit = false;
			lock (_vm) {
				exit = _exited;
			}
			if (exit)
				return;

			if (policy !=  MDS.SuspendPolicy.None)
				if (OnVMGotSuspended != null) OnVMGotSuspended (new Event(e));

			switch (e.EventType)
			{
				case MDS.EventType.VMStart:
					if (OnVMStart != null) OnVMStart (new Event(e));
					break;
				case MDS.EventType.ThreadStart:
					if (OnThreadStart != null)
						OnThreadStart (new Event(e));
					break;
				case MDS.EventType.AssemblyLoad:
					if (OnAssemblyLoad != null)
						OnAssemblyLoad (new AssemblyLoadEvent(e));
					break;
				case MDS.EventType.TypeLoad:
					if (OnTypeLoad != null)
						OnTypeLoad (new TypeLoadEvent(e));
					break;
				case MDS.EventType.Breakpoint:
					if (OnBreakpoint != null)
						OnBreakpoint (new Event(e));
					break;
				case MDS.EventType.VMDeath:
					if (OnVMDeath != null) OnVMDeath (new Event(e));
					_running = false;
					break;
				case MDS.EventType.VMDisconnect:
					if (OnVMDisconnect != null) OnVMDisconnect (new Event(e));
					_running = false;
					break;
				case MDS.EventType.MethodEntry:
					Console.WriteLine (((MDS.MethodEntryEvent)e).Method.FullName);
					break;
				default:
					Console.WriteLine ("Unknown event: "+e.GetType ());
					break;
			}
			if (policy != MDS.SuspendPolicy.None)
				_vm.Resume();
		}

		private void WithErrorLogging (Action action)
		{
			try
			{
				action ();
			}
			catch (Exception e)
			{
				TraceError (e);
				_errors.Add (e);
			}
		}

		private void TraceError (Exception exception)
		{
			Console.WriteLine (exception.ToString ());
		}

		public void Exit ()
		{
			lock (_vm) {
				_exited = true;
			}
			_vm.Exit (0);
			_vm.Detach ();
		}

		public IBreakpointEventRequest CreateBreakpointRequest (ILocation location)
		{
			return new SdbBreakpointEventRequest(_vm.CreateBreakpointRequest (location.Unwrap<MDS.Location>()));
		}

		public void ResumeIfNeeded ()
		{
		}

		public IList<IThreadMirror> GetThreads ()
		{
			//return _vm.GetThreads();
			return null;
		}
	}
}
