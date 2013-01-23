using System;
using System.Collections.Generic;
using System.Linq;
using Debugger.Backend;

namespace Debugger
{
	public class BreakpointMediator
	{
		private readonly DebuggerSession session;
		private readonly Dictionary<IBreakpoint, IBreakpoint> breakpoints = new Dictionary<IBreakpoint, IBreakpoint> ();

		public BreakpointMediator (DebuggerSession session)
		{
			this.session = session;

			session.TypeProvider.TypeLoaded += OnTypeLoaded;
			session.TypeProvider.TypeUnloaded += OnTypeUnloaded;
			session.BreakpointProvider.BreakpointAdded += OnBreakpointAdded;
			session.BreakpointProvider.BreakpointRemoved += OnBreakpointRemoved;
			session.BreakpointProvider.BreakpointEnabled += OnBreakpointEnabled;
			session.BreakpointProvider.BreakpointDisabled += OnBreakpointDisabled;
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
			var types = session.SourceProvider.TypesFor (breakpoint.Location.SourceFile);
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

		private void OnTypeLoaded (ITypeEvent ev, ITypeMirror type)
		{
			if (ev.Cancel)
				return;

			var sourcefiles = type.SourceFiles;
			var relevantBreakpoints = session.BreakpointProvider.Breakpoints.Where (bp => sourcefiles.Contains (bp.Location.SourceFile));

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
			var locations = method.Locations;
			//var name = method.FullName;

			return locations.FirstOrDefault (l => l.SourceFile == bp.Location.SourceFile && l.LineNumber == bp.Location.LineNumber);
		}
	}
}
