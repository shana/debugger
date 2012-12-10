using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CodeEditor.Composition;
using Debugger.Backend;
using Mono.Debugger.Soft;
using MDS=Mono.Debugger.Soft;


namespace CodeEditor.Debugger.Implementation
{
	
	[Export(typeof(IVirtualMachine))]
	internal class VirtualMachine : IVirtualMachine
	{
		private readonly MDS.VirtualMachine _vm;
		private bool _running = true;
		private bool _exited = false;
		private readonly List<Exception> _errors = new List<Exception> ();

		public event Action<VMStartEvent> OnVMStart;
		public event Action<VMDeathEvent> OnVMDeath;
		public event Action<AssemblyLoadEvent> OnAssemblyLoad;

		public event Action<MDS.TypeLoadEvent> OnTypeLoad;
		public event Action<VMDisconnectEvent> OnVMDisconnect;
		public event Action<ThreadStartEvent> OnThreadStart;
		public event Action<BreakpointEvent> OnBreakpoint;
		public event Action<MDS.Event> OnVMGotSuspended;

		public VirtualMachine()
		{
		}

		public VirtualMachine(MDS.VirtualMachine vm)
		{
			_vm = vm;
			_vm.EnableEvents (
				EventType.AssemblyLoad,
				EventType.VMDeath,
				EventType.TypeLoad,
				EventType.VMStart
			);

			QueueUserWorkItem (EventLoop);
		}

		public Process Process
		{
			get { return _vm.Process; }
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

		private void HandleEvent (MDS.Event e, SuspendPolicy policy)
		{
			Console.WriteLine("Event: " + e.GetType());
			bool exit = false;
			lock (_vm) {
				exit = _exited;
			}
			if (exit)
				return;

			if (policy !=  SuspendPolicy.None)
				if (OnVMGotSuspended != null) OnVMGotSuspended (e);

			switch (e.EventType)
			{
				case EventType.VMStart:
					if (OnVMStart !=null) OnVMStart ((VMStartEvent) e);
					break;
				case EventType.ThreadStart:
					if (OnThreadStart != null)
						OnThreadStart ((ThreadStartEvent)e);
					break;
				case EventType.AssemblyLoad:
					if (OnAssemblyLoad != null)
						OnAssemblyLoad ((AssemblyLoadEvent)e);
					break;
				case EventType.TypeLoad:
					if (OnTypeLoad != null)
						OnTypeLoad ((MDS.TypeLoadEvent)e);
					break;
				case EventType.Breakpoint:
					if (OnBreakpoint != null)
						OnBreakpoint ((BreakpointEvent)e);
					break;
				case EventType.VMDeath:
					if (OnVMDeath != null) OnVMDeath ((VMDeathEvent) e);
					_running = false;
					break;
				case EventType.VMDisconnect:
					if (OnVMDisconnect != null) OnVMDisconnect ((VMDisconnectEvent)e);
					_running = false;
					break;
				case EventType.MethodEntry:
					Console.WriteLine (((MethodEntryEvent)e).Method.FullName);
					break;
				default:
					Console.WriteLine ("Unknown event: "+e.GetType ());
					break;
			}
			if (policy != SuspendPolicy.None)
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

		public BreakpointEventRequest CreateBreakpointRequest (Mono.Debugger.Soft.Location location)
		{
			return _vm.CreateBreakpointRequest (location);
		}

		public void ResumeIfNeeded ()
		{
		}

		public IList<ThreadMirror> GetThreads ()
		{
			return _vm.GetThreads();
		}
	}
}
