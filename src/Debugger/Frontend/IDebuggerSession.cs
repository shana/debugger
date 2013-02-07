using System;
using System.Collections.Generic;
using Debugger.Backend;

namespace Debugger
{
	public interface IDebuggerSession
	{
		int Port { get; set; }
		bool Active { get; }
		IThreadMirror MainThread { get; }
		IVirtualMachine VM { get; }
		IExecutionProvider ExecutionProvider { get; }
		IThreadProvider ThreadProvider { get; }
		ITypeProvider TypeProvider { get; }
		IBreakpointProvider BreakpointProvider { get; }

		event Action<string> TraceCallback;

		void Start ();
		void Stop ();
	}
}
