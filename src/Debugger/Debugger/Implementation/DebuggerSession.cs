using System;
using Debugger.Backend;

namespace Debugger.Implementation
{
	public class DebuggerSession : IDebuggerSession
	{
		public bool Active { get; private set; }

		public IVirtualMachine VM { get; private set; }

		public static IDebuggerSession Attach (int pid)
		{
			return new DebuggerSession();
		}

		public static IDebuggerSession Launch ()
		{
			return new DebuggerSession();
		}

		public event Action<string> TraceCallback;
	}
}
