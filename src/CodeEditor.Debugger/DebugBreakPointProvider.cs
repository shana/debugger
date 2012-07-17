using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger
{
	public interface IDebugBreakPointProvider
	{
		IBreakPoint GetBreakPointAt(string file, int lineNumber);
		void ToggleBreakPointAt(string fileName, int lineNumber);
	}

	[Export(typeof(IDebugBreakPointProvider))]
	class DebugBreakPointProvider : IDebugBreakPointProvider
	{
		readonly List<IBreakPoint> _breakPoints = new List<IBreakPoint>();

		public IBreakPoint GetBreakPointAt(string file, int lineNumber)
		{
			return _breakPoints.FirstOrDefault(bp => bp.File == file && bp.LineNumber == lineNumber);
		}

		public void ToggleBreakPointAt(string fileName, int lineNumber)
		{
			var breakPoint = GetBreakPointAt(fileName, lineNumber);
			if (breakPoint == null)
				_breakPoints.Add(new BreakPoint(fileName, lineNumber));
			else
				_breakPoints.Remove(breakPoint);
		}
	}
}
