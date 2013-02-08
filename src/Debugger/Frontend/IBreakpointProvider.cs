using System;
using System.Collections.Generic;
using Debugger.Backend;

namespace Debugger
{
	public interface IBreakpointProvider
	{
		IDictionary<IBreakpoint, IBreakpoint> Breakpoints { get; }
		IBreakpoint GetBreakpointAt(string file, int line);
		void ToggleBreakpointAt(string file, int line);
		IBreakpoint AddBreakpoint (string file, int line);
		bool RemoveBreakpoint (IBreakpoint breakpoint);
		bool RemoveBreakpoint (string file, int line);

		IBreakpoint this [int index] { get; }
		event Action<IBreakpoint> BreakpointBound;
	}
}
