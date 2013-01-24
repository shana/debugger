using System;
using System.Collections.Generic;
using Debugger.Backend;

namespace Debugger
{
	public interface IBreakpointProvider
	{
		IBreakpoint GetBreakpointAt(string file, int lineNumber);
		void ToggleBreakpointAt(string fileName, int lineNumber);
		IEnumerable<IBreakpoint> Breakpoints { get; }
		bool AddBreakpoint (string file, int lineNumber);
		bool RemoveBreakpoint (IBreakpoint breakpoint);
		bool RemoveBreakpoint (string file, int lineNumber);

		event Action<IBreakpoint> BreakpointAdded;
		event Action<IBreakpoint> BreakpointRemoved;
		event Action<IBreakpoint> BreakpointEnabled;
		event Action<IBreakpoint> BreakpointDisabled;
	}
}
