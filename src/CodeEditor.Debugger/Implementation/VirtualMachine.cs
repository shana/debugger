using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CodeEditor.Composition;
using Mono.Debugger.Soft;
using MDS=Mono.Debugger.Soft;


namespace CodeEditor.Debugger.Implementation
{
	internal class VirtualMachine
	{
		private readonly MDS.VirtualMachine _vm;
		private bool _running = true;
		private readonly List<Exception> _errors = new List<Exception>();

		public event Action<VMStartEvent> OnVMStart;
		public event Action<VMDeathEvent> OnVMDeath;
		public event Action<AssemblyLoadEvent> OnAssemblyLoad;
		public event Action<TypeLoadEvent> OnTypeLoad;
		public event Action<VMDisconnectEvent> OnVMDisconnect;
		public event Action<ThreadStartEvent> OnThreadStart;

		public VirtualMachine(MDS.VirtualMachine vm)
		{
			_vm = vm;
			_vm.EnableEvents(
				EventType.AssemblyLoad,
				EventType.VMDeath,
				EventType.TypeLoad,
				EventType.VMStart
				);

			QueueUserWorkItem(EventLoop);
		}

		public Process Process
		{
			get { return _vm.Process; }
		}

		public void Resume()
		{
			_vm.Resume();
		}

		public IEnumerable<Exception> Errors {
			get { return _errors; }
		}

		private void QueueUserWorkItem(Action a)
		{
			ThreadPool.QueueUserWorkItem(_ => WithErrorLogging(a));
		}

		private void EventLoop()
		{
			while(_running)
			{
				var e = _vm.GetNextEvent();
				if (e == null)
					return;
				HandleEvent(e);
			}
		}

		private void HandleEvent(Event e)
		{
			Console.WriteLine("Event: "+e.GetType());
			switch (e.EventType)
			{
				case EventType.VMStart:
					if (OnVMStart !=null) OnVMStart((VMStartEvent) e);
					return;
				case EventType.ThreadStart:
					if (OnThreadStart != null) OnThreadStart((ThreadStartEvent) e);
					return;
				case EventType.AssemblyLoad:
					if (OnAssemblyLoad != null) OnAssemblyLoad((AssemblyLoadEvent)e);
					return;
				case EventType.TypeLoad:
					if (OnTypeLoad != null) OnTypeLoad((TypeLoadEvent)e);
					return;
				case EventType.VMDeath:
					if (OnVMDeath != null) OnVMDeath((VMDeathEvent) e);
					_running = false;
					return;
				case EventType.VMDisconnect:
					if (OnVMDisconnect != null) OnVMDisconnect((VMDisconnectEvent)e);
					_running = false;
					return;
				case EventType.MethodEntry:
					Console.WriteLine(((MethodEntryEvent)e).Method.FullName);
					return;
				default:
					Console.WriteLine("Unknown event: "+e.GetType());
					return;
			}
		}

		private void WithErrorLogging(Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				TraceError(e);
				_errors.Add(e);
			}
		}

		private void TraceError(Exception exception)
		{
			Console.WriteLine(exception.ToString());
		}

		public void Exit()
		{
			_vm.Exit(0);
		}
	}
}
