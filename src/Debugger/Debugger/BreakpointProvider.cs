using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Backend;

namespace Debugger
{
	[Export (typeof (IBreakpointProvider))]
	public class BreakpointProvider : IBreakpointProvider
	{
		private readonly ITypeProvider typeProvider;
		private readonly Dictionary<IBreakpoint, IBreakpoint> breakpoints = new Dictionary<IBreakpoint, IBreakpoint> ();
		public IList<IBreakpoint> Breakpoints { get { return new ReadOnlyCollection<IBreakpoint> (breakpoints.Keys.ToList ());}}

		public event Action<IBreakpoint> BreakpointAdded;
		public event Action<IBreakpoint> BreakpointRemoved;

		public event Action<IBreakpoint, IBreakpoint, ILocation> BreakpointBound;
		public event Action<IBreakpoint, IBreakpoint, ILocation> BreakpointUnbound;


		public IBreakpoint this[int index] { get { return breakpoints.Keys.ElementAt (index); } }

		[ImportingConstructor]
		public BreakpointProvider (ITypeProvider typeProvider)
		{
			this.typeProvider = typeProvider;
			typeProvider.TypeLoaded += OnTypeLoaded;
			typeProvider.TypeUnloaded += OnTypeUnloaded;
			Breakpoint.OnEnable += breakpoint => {
					IBreakpoint bound;
					if (breakpoints.TryGetValue (breakpoint, out bound))
						bound.Enable ();
			};
			Breakpoint.OnDisable += breakpoint => {
					IBreakpoint bound;
					if (breakpoints.TryGetValue (breakpoint, out bound))
						bound.Disable ();
			};
		}

		public IEnumerable<ILocation> GetBoundLocations (IBreakpoint breakpoint)
		{
			return breakpoints.Where (x => x.Key == breakpoint).Select (b => b.Value.Location as ILocation);
		}

		public IEnumerable<IBreakpoint> GetBreakpoints (bool bound)
		{
			if (bound)
				return breakpoints.Where (x => x.Value != null).Select (b => b.Key);
			return breakpoints.Keys;
		}

		public IBreakpoint GetBreakpointAt (string file, int line)
		{
			file = typeProvider.MapFullPath (file);
			return breakpoints.Keys.FirstOrDefault (bp => bp.Location.SourceFile == file && bp.Location.LineNumber == line);
		}

		public int IndexOf (IBreakpoint breakpoint)
		{
			int i  = 0;
			var keys = breakpoints.Keys.ToArray ();
			for (i = 0; i < keys.Length; i++)
				if (keys[i] == breakpoint)
					return i;
			return -1;
		}

		public void ToggleBreakpointAt (string file, int line)
		{
			//LogProvider.Log ("Toggling breakpoint at file {0} line {1}", file, line);

			file = typeProvider.MapFullPath (file);

			var breakPoint = GetBreakpointAt (file, line);
			if (breakPoint == null)
				AddBreakpoint (new Breakpoint (new Location (file, line)));
			else
				RemoveBreakpoint (breakPoint);
		}

		private bool AddBreakpoint (IBreakpoint breakpoint)
		{
			if (breakpoints.ContainsKey (breakpoint))
				return false;

			breakpoints.Add (breakpoint, null);
			breakpoint.Enable ();
			if (BreakpointAdded != null)
				BreakpointAdded (breakpoint);

			foreach(var type in typeProvider.TypesFor (breakpoint.Location.SourceFile))
				if (BindBreakpoint (type, breakpoint))
					break;

			return true;
		}

		public IBreakpoint AddBreakpoint (string file, int line)
		{
			file = typeProvider.MapFullPath (file);
			if (file == null)
				return null;
			var bp = new Breakpoint (new Location (file, line));
			if (AddBreakpoint (bp))
				return bp;
			return null;
		}

		public bool RemoveBreakpoint (IBreakpoint breakpoint)
		{
			if (!breakpoints.ContainsKey (breakpoint))
				return false;
			IBreakpoint bp;
			if (breakpoints.TryGetValue (breakpoint, out bp) && bp.Enabled)
				breakpoint.Disable ();

			if (BreakpointRemoved != null)
				BreakpointRemoved (breakpoint);
			breakpoints.Remove (breakpoint);
			return true;
		}

		public bool IsBound (IBreakpoint breakpoint)
		{
			IBreakpoint val = null;
			return breakpoints.TryGetValue (breakpoint, out val) && val != null;
		}

		public ILocation GetBoundLocation (IBreakpoint breakpoint)
		{
			IBreakpoint val = null;
			return breakpoints.TryGetValue (breakpoint, out val) && val != null ? val.Location : null;
		}

		public bool RemoveBreakpoint (string file, int line)
		{
			file = typeProvider.MapFullPath (file);

			var breakpoint = GetBreakpointAt (file, line);
			if (breakpoint != null)
				return RemoveBreakpoint (breakpoint);
			return false;
		}

		private void OnTypeLoaded (ITypeMirror type)
		{
			var sourcefiles = type.SourceFiles;
			var relevantBreakpoints = Breakpoints.Where (bp => sourcefiles.Contains (bp.Location.SourceFile));

			foreach (var bp in relevantBreakpoints)
				BindBreakpoint (type, bp);
		}

		private bool BindBreakpoint (ITypeMirror type, IBreakpoint bp)
		{
			foreach (var method in type.Methods)
			{
				var bestLocation = BestLocationIn (method, bp);
				if (bestLocation == null)
					continue;

				var b = Factory.CreateBreakpoint (bestLocation);
				breakpoints[bp] = b;
				if (bp.Enabled) b.Enable ();

				if (BreakpointBound != null)
						BreakpointBound (bp, b, b.Location);
				return true;
			}
			return false;
		}

		private void OnTypeUnloaded (ITypeMirror typeMirror)
		{
			var bps = breakpoints.Where (x => typeMirror.SourceFiles.Contains(x.Value.Location.SourceFile)).ToArray ();
			foreach (var bp in bps) {
				if (BreakpointUnbound != null)
					BreakpointUnbound (bp.Key, bp.Value, bp.Value.Location);
				breakpoints.Remove (bp.Key);
			}
		}

		private ILocation BestLocationIn (IMethodMirror method, IBreakpoint bp)
		{
			return method.Locations.FirstOrDefault (l => l.SourceFile == bp.Location.SourceFile && l.LineNumber == bp.Location.LineNumber);
		}
	}
}
