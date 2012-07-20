using System;
using System.Diagnostics;
using System.Threading;
using CodeEditor.Composition;
using Mono.Debugger.Soft;
using MDS=Mono.Debugger.Soft;


namespace CodeEditor.Debugger.Implementation
{
	[Export(typeof(IVirtualMachine))]
	class VirtualMachine : IVirtualMachine
	{
		private readonly MDS.VirtualMachine _vm;
		private bool _running = true;

		public event Action<VMStartEvent> OnVMStart;
		public event Action<VMDeathEvent> OnVMDeath;
		public event Action<TypeLoadEvent> OnTypeLoad;
		public event Action<VMDisconnectEvent> OnVMDisconnect;
		public event Action<ThreadStartEvent> OnThreadStart;

		public VirtualMachine(MDS.VirtualMachine vm)
		{
			_vm = vm;
			_vm.EnableEvents(
				EventType.AssemblyLoad,
				//EventType.AssemblyUnload,
				//EventType.AppDomainUnload,
				//EventType.AppDomainCreate,
				EventType.VMDeath,
				EventType.VMDisconnect,
				//EventType.TypeLoad,
				EventType.VMStart
				//EventType.ThreadStart
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
			switch (e.EventType)
			{
				case EventType.VMStart:
					if (OnVMStart !=null) OnVMStart((VMStartEvent) e);
					return;
				case EventType.ThreadStart:
					if (OnThreadStart != null) OnThreadStart((ThreadStartEvent) e);
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
				TraceError(e.ToString());
			}
		}

		private void TraceError(string error)
		{
			Console.WriteLine(error);
		}

		public void Exit()
		{
			_vm.Exit(0);
		}
	}
}
