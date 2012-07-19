using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;

namespace CodeEditor.Debugger.Implementation
{
	[Export(typeof(IDebuggerSessionCreationListener))]
	class BreakPointMediatorFactory : IDebuggerSessionCreationListener
	{
		[Import]
		private IDebugBreakPointProvider DebugBreakPointProvider { get; set; }
		
		[Import]
		private IDebugTypeProvider DebugTypeProvider { get; set; }

		[Import]
		private BreakpointEventRequestFactory BreakpointEventRequestFactory { get; set; }

		public void OnCreate(IDebuggerSession session)
		{
			new BreakpointMediator(DebugBreakPointProvider, DebugTypeProvider, BreakpointEventRequestFactory);
		}
	}
	
	class BreakpointMediator
	{
		private readonly IDebugBreakPointProvider _debugBreakPointProvider;
		private readonly IDebugTypeProvider _debugTypeProvider;
		private readonly IBreakpointEventRequestFactory _breakpointEventRequestFactory;
		private readonly List<IBreakPoint> _breakPoints = new List<IBreakPoint>();

		public BreakpointMediator(IDebugBreakPointProvider debugBreakPointProvider, IDebugTypeProvider debugTypeProvider, IBreakpointEventRequestFactory breakpointEventRequestFactory)
		{
			_debugBreakPointProvider = debugBreakPointProvider;
			_debugTypeProvider = debugTypeProvider;
			_breakpointEventRequestFactory = breakpointEventRequestFactory;
			_debugBreakPointProvider.BreakpointAdded += BreakpointAdded;
			_debugTypeProvider.TypeLoaded += TypeLoaded;
		}

		private void TypeLoaded(IDebugType type)
		{
			foreach (var breakpoint in type.SourceFiles.SelectMany(BreakPointsIn))
			{
				IBreakPoint breakpoint1 = breakpoint;
				var locations = type.Methods.SelectMany(m => m.Locations).Where(l => LocationsMatch(l, breakpoint1));
				foreach(var location in locations)
					CreateEventRequest(location);
			}
		}

		private IEnumerable<IBreakPoint> BreakPointsIn(string file)
		{
			return _breakPoints.Where(b => b.File == file);
		}

		private bool LocationsMatch(IDebugLocation location, IBreakPoint breakpoint)
		{
			return breakpoint.File == location.File;
		}

		private static bool DoesTypeHaveCodeIn(IDebugType type, string sourceFile)
		{
			return type.SourceFiles.Contains(sourceFile);
		}

		private void BreakpointAdded(IBreakPoint breakpoint)
		{
			_breakPoints.Add(breakpoint);

			foreach (var type in TypesWithCodeIn(breakpoint.File))
			{
				var locationInMethod = type.Methods.SelectMany(m => m.Locations).FirstOrDefault(l => LocationsMatch(l, breakpoint));
				if (locationInMethod == null)
					continue;
				CreateEventRequest(locationInMethod);
			}
		}

		private IEnumerable<IDebugType> TypesWithCodeIn(string sourceFile)
		{
			return _debugTypeProvider.LoadedTypes.Where(t => DoesTypeHaveCodeIn(t, sourceFile));
		}

		private void CreateEventRequest(IDebugLocation locationInMethod)
		{
			var request = _breakpointEventRequestFactory.Create(locationInMethod);
			request.Enable();
		}
	}
}
