using System;
using System.Collections.Generic;
using Debugger.Backend;

namespace Debugger
{
	public interface IBreakpointProvider
	{
		IBreakPoint GetBreakPointAt(string file, int lineNumber);
		void ToggleBreakPointAt(string fileName, int lineNumber);
		event Action<IBreakPoint> BreakpointAdded;
		event Action<IBreakPoint> BreakPointRemoved;
		IEnumerable<IBreakPoint> Breakpoints { get; }
	}
}
