using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Backend;

namespace Debugger
{
	public class BreakpointMediator
	{
		private readonly ITypeProvider typeProvider;
		private readonly IBreakpointProvider breakpointProvider;
		private readonly Dictionary<IBreakpoint, IBreakpoint> breakpoints = new Dictionary<IBreakpoint, IBreakpoint> ();

		[ImportingConstructor]
		public BreakpointMediator (ITypeProvider typeProvider, IBreakpointProvider breakpointProvider)
		{
			this.typeProvider = typeProvider;
			this.breakpointProvider = breakpointProvider;
			typeProvider.TypeLoaded += OnTypeLoaded;
			typeProvider.TypeUnloaded += OnTypeUnloaded;
			breakpointProvider.BreakpointAdded += OnBreakpointAdded;
			breakpointProvider.BreakpointRemoved += OnBreakpointRemoved;
			breakpointProvider.BreakpointEnabled += OnBreakpointEnabled;
			breakpointProvider.BreakpointDisabled += OnBreakpointDisabled;
		}

		private void OnBreakpointEnabled (IBreakpoint breakpoint)
		{
			IBreakpoint bp;
			if (breakpoints.TryGetValue (breakpoint, out bp))
				bp.Enable ();
			else
				OnBreakpointAdded (breakpoint);
		}

		private void OnBreakpointDisabled (IBreakpoint breakpoint)
		{
			IBreakpoint bp;
			if (breakpoints.TryGetValue (breakpoint, out bp))
				bp.Disable ();
		}

		private void OnBreakpointRemoved (IBreakpoint breakpoint)
		{
			IBreakpoint bp;
			if (breakpoints.TryGetValue (breakpoint, out bp)) {
				bp.Disable ();
				breakpoints.Remove (breakpoint);
			}
		}

		private void OnBreakpointAdded (IBreakpoint breakpoint)
		{
			var types = typeProvider.TypesFor (breakpoint.Location.SourceFile);
			foreach (var type in types)
			{
				foreach (var method in type.Methods)
				{
					var bestLocation = BestLocationIn (method, breakpoint);
					if (bestLocation == null)
						continue;

					breakpoints.Add (breakpoint, Factory.CreateBreakpoint (bestLocation));
					break;
				}
			}
		}

		private void OnTypeLoaded (ITypeMirror type)
		{
			var sourcefiles = type.SourceFiles;
			var relevantBreakpoints = breakpointProvider.Breakpoints.Where (bp => sourcefiles.Contains (bp.Location.SourceFile));

			foreach (var bp in relevantBreakpoints)
			{
				foreach (var method in type.Methods)
				{
					var bestLocation = BestLocationIn (method, bp);
					if (bestLocation == null)
						continue;

					var b = Factory.CreateBreakpoint (bestLocation);
					breakpoints.Add (bp, b);
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
