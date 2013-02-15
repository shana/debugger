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

		public event Action<IBreakpoint, IBreakpoint, ILocation> BreakpointBound;
		public event Action<IBreakpoint, IBreakpoint, ILocation> BreakpointUnbound;


		public IBreakpoint this[int index] { get { return breakpoints.Keys.ElementAt (index); } }

		[ImportingConstructor]
		public BreakpointProvider (ITypeProvider typeProvider)
		{
			this.typeProvider = typeProvider;
			typeProvider.TypeLoaded += OnTypeLoaded;
			typeProvider.TypeUnloaded += OnTypeUnloaded;
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
			file = typeProvider.MapFile (file);
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
			LogProvider.Log ("Toggling breakpoint at line: " + line);

			file = typeProvider.MapFile (file);

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
			return true;
		}

		public IBreakpoint AddBreakpoint (string file, int line)
		{
			file = typeProvider.MapFile (file);
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
					if (BreakpointBound != null)
						BreakpointBound (bp.Key, b, b.Location);
					break;
				}
			}
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
