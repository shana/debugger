using System;
using System.Collections.Generic;
using System.IO;
using CodeEditor.Composition;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger
{
	public interface IDebugBreakPointProvider
	{
		IBreakPoint GetBreakPointAt(string file, int lineNumber);
		void ToggleBreakPointAt(string fullName, int lineNumber);
	}

	[Export(typeof(IDebugBreakPointProvider))]
	class DebugBreakPointProvider : IDebugBreakPointProvider
	{
		private readonly IBreakPoint _breakPoint = new BreakPoint();

		readonly List<int> _setBreakPoints = new List<int>();

		public IBreakPoint GetBreakPointAt(string file, int lineNumber)
		{
			return (_setBreakPoints.Contains(lineNumber) ? _breakPoint : null);
		}

		public void ToggleBreakPointAt(string fullName, int lineNumber)
		{
			if (GetBreakPointAt(fullName, lineNumber) == null)
				_setBreakPoints.Add(lineNumber);
			else
				_setBreakPoints.Remove(lineNumber);
		}
	}
}
