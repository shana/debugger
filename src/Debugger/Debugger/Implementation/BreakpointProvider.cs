﻿using System;
using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using Debugger;
using Debugger.Backend;

namespace Debugger.Implementation
{
	[Export(typeof(IBreakpointProvider))]
	class BreakpointProvider : IBreakpointProvider
	{
		readonly List<IBreakPoint> _breakPoints = new List<IBreakPoint>();

		public event Action<IBreakPoint> BreakpointAdded;
		public event Action<IBreakPoint> BreakPointRemoved;

		public IEnumerable<IBreakPoint> Breakpoints
		{
			get { return _breakPoints; }
		}

		public IBreakPoint GetBreakPointAt(string file, int lineNumber)
		{
			return _breakPoints.FirstOrDefault(bp => bp.Location.SourceFile == file && bp.Location.LineNumber == lineNumber);
		}

		public void ToggleBreakPointAt(string fileName, int lineNumber)
		{
			Console.WriteLine("Toggling breakpoint at line: "+lineNumber);
			var breakPoint = GetBreakPointAt(fileName, lineNumber);
			if (breakPoint == null)
				AddBreakPoint(new BreakPoint(new Location(lineNumber,fileName)));
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