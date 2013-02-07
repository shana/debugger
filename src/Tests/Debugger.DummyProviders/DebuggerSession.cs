using System;
using CodeEditor.Composition;
using Debugger.Backend;

namespace Debugger.DummyProviders
{
	[Export(typeof(IDebuggerSession))]
	public class DebuggerSession : IDebuggerSession
	{
		public int Port { get; set; }
		public bool Active { get; private set; }

		public IThreadMirror MainThread { get; set; }

		public IVirtualMachine VM { get; set; }
		public IExecutionProvider ExecutionProvider { get; set; }
		public IThreadProvider ThreadProvider { get; set; }
		public ITypeProvider TypeProvider { get; set; }
		public IBreakpointProvider BreakpointProvider { get; set; }

		[ImportingConstructor]
		public DebuggerSession (IVirtualMachine virtualMachine,
			IExecutionProvider executionProvider, ITypeProvider typeProvider,
			IThreadProvider threadProvider, IBreakpointProvider breakpointProvider)
		{
			VM = virtualMachine;
			ExecutionProvider = executionProvider;
			ThreadProvider = threadProvider;
			TypeProvider = typeProvider;
			BreakpointProvider = breakpointProvider;
		}

		public event Action<string> TraceCallback;

		public void Start ()
		{
			Active = true;
			
		}

		public void Stop ()
		{
			Active = false;
		}
	}
}
