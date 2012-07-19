using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Debugger.Backend;

namespace CodeEditor.Debugger.Implementation
{
	[Export(typeof(IDebuggerSessionCreationListener))]
	class BreakPointMediatorFactory : IDebuggerSessionCreationListener
	{
		[Import]
		private IBreakpointProvider BreakpointProvider { get; set; }
		
		[Import]
		private ITypeMirrorProvider TypeMirrorProvider { get; set; }

		[Import]
		private IBreakpointEventRequestFactory BreakpointEventRequestFactory { get; set; }

		public void OnCreate(IDebuggerSession session)
		{
			new BreakpointMediator(BreakpointProvider, TypeMirrorProvider, BreakpointEventRequestFactory);
		}
	}
	
	class BreakpointMediator
	{
		private readonly IBreakpointProvider _breakpointProvider;
		private readonly ITypeMirrorProvider _typeMirrorProvider;
		private readonly IBreakpointEventRequestFactory _breakpointEventRequestFactory;

		public BreakpointMediator(IBreakpointProvider breakpointProvider, ITypeMirrorProvider typeMirrorProvider, IBreakpointEventRequestFactory breakpointEventRequestFactory)
		{
			_breakpointProvider = breakpointProvider;
			_typeMirrorProvider = typeMirrorProvider;
			_breakpointEventRequestFactory = breakpointEventRequestFactory;
			_breakpointProvider.BreakpointAdded += BreakpointAdded;
			_typeMirrorProvider.TypeLoaded += TypeMirrorLoaded;
		}

		private void TypeMirrorLoaded(ITypeMirror typeMirror)
		{
			foreach (var breakpoint in typeMirror.SourceFiles.SelectMany(BreakPointsIn))
			{
				IBreakPoint breakpoint1 = breakpoint;
				var locations = typeMirror.Methods.SelectMany(m => m.Locations).Where(l => LocationsMatch(l, breakpoint1));
				foreach(var location in locations)
					CreateEventRequest(location);
			}
		}

		private IEnumerable<IBreakPoint> BreakPointsIn(string file)
		{
			return _breakpointProvider.Breakpoints.Where(b => b.File == file);
		}

		private bool LocationsMatch(ILocation location, IBreakPoint breakpoint)
		{
			return breakpoint.File == location.File;
		}

		private static bool DoesTypeHaveCodeIn(ITypeMirror typeMirror, string sourceFile)
		{
			return typeMirror.SourceFiles.Contains(sourceFile);
		}

		private void BreakpointAdded(IBreakPoint breakpoint)
		{
			foreach (var type in TypesWithCodeIn(breakpoint.File))
			{
				var locationInMethod = type.Methods.SelectMany(m => m.Locations).FirstOrDefault(l => LocationsMatch(l, breakpoint));
				if (locationInMethod == null)
					continue;
				CreateEventRequest(locationInMethod);
			}
		}

		private IEnumerable<ITypeMirror> TypesWithCodeIn(string sourceFile)
		{
			return _typeMirrorProvider.LoadedTypesMirror.Where(t => DoesTypeHaveCodeIn(t, sourceFile));
		}

		private void CreateEventRequest(ILocation locationInMethod)
		{
			var request = _breakpointEventRequestFactory.Create(locationInMethod);
			request.Enable();
		}
	}
}
