using System;
using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;

namespace CodeEditor.Debugger.Implementation
{
	[Export(typeof(IDebugBreakPointProvider))]
	class DebugBreakPointProvider : IDebugBreakPointProvider
	{
		readonly List<IBreakPoint> _breakPoints = new List<IBreakPoint>();

		public event Action<IBreakPoint> BreakpointAdded;
		public event Action<IBreakPoint> BreakPointRemoved;

		public IBreakPoint GetBreakPointAt(string file, int lineNumber)
		{
			return _breakPoints.FirstOrDefault(bp => bp.File == file && bp.LineNumber == lineNumber);
		}

		public void ToggleBreakPointAt(string fileName, int lineNumber)
		{
			var breakPoint = GetBreakPointAt(fileName, lineNumber);
			if (breakPoint == null)
				AddBreakPoint(new BreakPoint(fileName, lineNumber));
			else
				RemoveBreakPoint(breakPoint);
		}

		private void AddBreakPoint(IBreakPoint point)
		{
			_breakPoints.Add(point);
			if (BreakpointAdded != null)
				BreakpointAdded(point);
		}

		private void RemoveBreakPoint(IBreakPoint breakPoint)
		{
			_breakPoints.Remove(breakPoint);
			if (BreakPointRemoved != null)
				BreakPointRemoved(breakPoint);
		}
	}
}
