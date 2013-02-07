using System;
using CodeEditor.Composition;
using Debugger.Backend;

namespace Debugger.DummyProviders
{
	[Export(typeof(IExecutionProvider))]
	public class ExecutionProvider : IExecutionProvider
	{
		private readonly IVirtualMachine virtualMachine;

		public IThreadMirror CurrentThread { get; set; }
		public ILocation Location { get; set; }
		public bool Running { get; private set; }
		public event Action Break;
		public event Action<IThreadMirror> Suspended;

		[ImportingConstructor]
		public ExecutionProvider (IVirtualMachine virtualMachine)
		{
			this.virtualMachine = virtualMachine;
			this.virtualMachine.VMSuspended += VMSuspended;
		}

		private void VMSuspended (IEvent ev)
		{
			Running = false;
		}

		public void Resume ()
		{
			Running = true;
		}

		public void Step ()
		{
			
		}
	}
}
