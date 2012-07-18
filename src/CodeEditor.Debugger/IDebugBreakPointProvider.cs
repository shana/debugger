using System;

namespace CodeEditor.Debugger
{
	public interface IDebugBreakPointProvider
	{
		IBreakPoint GetBreakPointAt(string file, int lineNumber);
		void ToggleBreakPointAt(string fileName, int lineNumber);
		event Action<IBreakPoint> BreakPointAdded;
		event Action<IBreakPoint> BreakPointRemoved;
	}
}