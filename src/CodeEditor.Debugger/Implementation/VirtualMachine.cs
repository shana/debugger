using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public VirtualMachine(MDS.VirtualMachine vm)
		{
			_vm = vm;

			_vm.Suspend();
			_vm.EnableEvents(
				//EventType.AssemblyLoad,
				//EventType.AssemblyUnload,
				//EventType.AppDomainUnload,
				//EventType.AppDomainCreate,
				//EventType.VMDeath,
				//EventType.VMDisconnect,
				//EventType.TypeLoad
				EventType.VMStart
				);
			QueueUserWorkItem(EventLoop);
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
			int cycle = 0;
			while(_running)
			{
				var e = _vm.GetNextEvent();
				Console.WriteLine("Eventloop cycle " + (cycle++)+" e="+ ((e==null) ? "null" : e.GetType().ToString()));
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
					if (OnVMStart!=null) OnVMStart((VMStartEvent) e);
					return;
				case EventType.VMDeath:
					_running = false;
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
	}
}
