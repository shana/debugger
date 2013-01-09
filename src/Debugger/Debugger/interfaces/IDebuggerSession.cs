using System;
using Debugger.Backend;

namespace Debugger
{
	public interface IDebuggerSession
	{
		bool Active { get; }
		IVirtualMachine VM { get; }

		event Action<string> TraceCallback;
	}
}
