using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Backend;
using Moq;

namespace Debugger.DummyProviders
{
	[Export (typeof (IBreakpointProvider))]
	public class BreakpointProvider : IBreakpointProvider
	{
		private readonly ITypeProvider typeProvider;
		private readonly Dictionary<IBreakpoint, IBreakpoint> breakpoints = new Dictionary<IBreakpoint, IBreakpoint> ();
		public IDictionary<IBreakpoint, IBreakpoint> Breakpoints { get { return breakpoints; }} 

		[ImportingConstructor]
		public BreakpointProvider (ITypeProvider typeProvider)
		{
			this.typeProvider = typeProvider;
			typeProvider.TypeLoaded += OnTypeLoaded;
			typeProvider.TypeUnloaded += OnTypeUnloaded;
		}

		public IBreakpoint this[int index] { get { return breakpoints.Keys.ElementAt (index); } }

		public IBreakpoint GetBreakpointAt (string file, int line)
		{
			file = typeProvider.MapFile (file);
			return breakpoints.Keys.FirstOrDefault (bp => bp.Location.SourceFile == file && bp.Location.LineNumber == line);
		}

		public void ToggleBreakpointAt (string file, int line)
		{
			file = typeProvider.MapFile (file);

			var breakPoint = GetBreakpointAt (file, line);
			if (breakPoint == null)
				AddBreakpoint (new Mock<IBreakpoint>(new Mock<ILocation>(file, line).Object).Object);
			else
				RemoveBreakpoint (breakPoint);
		}

		private bool AddBreakpoint (IBreakpoint breakpoint)
		{
			if (breakpoints.ContainsKey (breakpoint))
				return false;
			breakpoints.Add (breakpoint, null);
			return true;
		}

		public IBreakpoint AddBreakpoint (string file, int line)
		{
			file = typeProvider.MapFile (file);
			if (file == null)
				return null;
			var bp = new Breakpoint (new Location (line, file));
			if (AddBreakpoint (bp))
				return bp;
			return null;
		}

		public bool RemoveBreakpoint (IBreakpoint breakpoint)
		{
			if (!breakpoints.ContainsKey (breakpoint))
				return false;

			breakpoints.Remove (breakpoint);
			return true;
		}

		public bool RemoveBreakpoint (string file, int line)
		{
			file = typeProvider.MapFile (file);

			var breakpoint = GetBreakpointAt (file, line);
			if (breakpoint != null)
				return RemoveBreakpoint (breakpoint);
			return false;
		}

		private void OnTypeLoaded (ITypeMirror type)
		{
			var sourcefiles = type.SourceFiles;
			var relevantBreakpoints = breakpoints.Where (bp => sourcefiles.Contains (bp.Key.Location.SourceFile));

			foreach (var bp in relevantBreakpoints)
			{
				foreach (var method in type.Methods)
				{
					var bestLocation = BestLocationIn (method, bp.Key);
					if (bestLocation == null)
						continue;

					var b = Factory.CreateBreakpoint (bestLocation);
					breakpoints[bp.Key] = b;
					b.Enable ();
					break;
				}
			}
		}

		private void OnTypeUnloaded (ITypeMirror typeMirror)
		{
			var bps = breakpoints.Where (x => typeMirror.SourceFiles.Contains(x.Value.Location.SourceFile)).ToArray ();
			foreach (var bp in bps)
				breakpoints.Remove (bp.Key);
		}

		private ILocation BestLocationIn (IMethodMirror method, IBreakpoint bp)
		{
			return method.Locations.FirstOrDefault (l => l.SourceFile == bp.Location.SourceFile && l.LineNumber == bp.Location.LineNumber);
		}

	}
}
