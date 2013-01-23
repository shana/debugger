using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Backend;

namespace Debugger
{
	[Export (typeof (IBreakpointProvider))]
	public class BreakpointProvider : IBreakpointProvider
	{
		private readonly ISourceProvider sourceProvider;
		readonly List<IBreakpoint> breakpoints = new List<IBreakpoint> ();

		public event Action<IBreakpoint> BreakpointAdded;
		public event Action<IBreakpoint> BreakpointRemoved;
		public event Action<IBreakpoint> BreakpointEnabled;
		public event Action<IBreakpoint> BreakpointDisabled;

		[ImportingConstructor]
		public BreakpointProvider (ISourceProvider sourceProvider)
		{
			this.sourceProvider = sourceProvider;
			Breakpoint.OnEnable += OnBreakpointEnabled;
			Breakpoint.OnDisable += OnBreakpointDisabled;
		}

		private void OnBreakpointEnabled (IBreakpoint breakpoint)
		{
			if (BreakpointEnabled != null)
				BreakpointEnabled (breakpoint);
		}

		private void OnBreakpointDisabled (IBreakpoint breakpoint)
		{
			if (BreakpointDisabled != null)
				BreakpointDisabled (breakpoint);
		}

		public IEnumerable<IBreakpoint> Breakpoints
		{
			get { return breakpoints; }
		}

		public IBreakpoint GetBreakpointAt (string file, int lineNumber)
		{
			if (!Path.IsPathRooted (file))
				file = sourceProvider.GetFullPathForFilename (file);
			return breakpoints.FirstOrDefault (bp => bp.Location.SourceFile == file && bp.Location.LineNumber == lineNumber);
		}

		public void ToggleBreakpointAt (string fileName, int lineNumber)
		{
			LogProvider.Log ("Toggling breakpoint at line: " + lineNumber);

			if (!Path.IsPathRooted (fileName))
				fileName = sourceProvider.GetFullPathForFilename (fileName);

			var breakPoint = GetBreakpointAt (fileName, lineNumber);
			if (breakPoint == null)
			{
				breakPoint = new Breakpoint (new Location (lineNumber, fileName));
				AddBreakpoint (breakPoint);
			}
			else
				RemoveBreakpoint (breakPoint);
		}

		private bool AddBreakpoint (IBreakpoint breakpoint)
		{
			if (breakpoints.Contains (breakpoint))
				return false;
			breakpoints.Add (breakpoint);
			if (BreakpointAdded != null)
				BreakpointAdded (breakpoint);
			return true;
		}

		public bool AddBreakpoint (string file, int lineNumber)
		{
			if (!Path.IsPathRooted (file))
				file = sourceProvider.GetFullPathForFilename (file);
			if (file == null)
				return false;
			return AddBreakpoint (new Breakpoint (new Location (lineNumber, file)));
		}

		public bool RemoveBreakpoint (IBreakpoint breakpoint)
		{
			if (!breakpoints.Contains (breakpoint))
				return false;

			breakpoints.Remove (breakpoint);
			if (BreakpointRemoved != null)
				BreakpointRemoved (breakpoint);
			return true;
		}

		public bool RemoveBreakpoint (string file, int lineNumber)
		{
			if (!Path.IsPathRooted (file))
				file = sourceProvider.GetFullPathForFilename (file);

			var breakpoint = GetBreakpointAt (file, lineNumber);
			if (breakpoint != null)
				return RemoveBreakpoint (breakpoint);
			return false;
		}

	}
}
