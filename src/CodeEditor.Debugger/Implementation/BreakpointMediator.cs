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

		public void OnCreate(IDebuggerSession session)
		{
			new BreakpointMediator(session,DebugBreakPointProvider, DebugTypeProvider);
		}
	}
	
	class BreakpointMediator
	{
		private readonly IDebuggerSession _session;
		private readonly IDebugBreakPointProvider _debugBreakPointProvider;
		private readonly IDebugTypeProvider _debugTypeProvider;
		private readonly List<IBreakPoint> _breakPoints = new List<IBreakPoint>();

		public BreakpointMediator(IDebuggerSession session, IDebugBreakPointProvider debugBreakPointProvider, IDebugTypeProvider debugTypeProvider)
		{
			_session = session;
			_debugBreakPointProvider = debugBreakPointProvider;
			_debugTypeProvider = debugTypeProvider;
			_debugBreakPointProvider.BreakpointAdded += BreakpointAdded;
			_debugTypeProvider.TypeLoaded += TypeLoaded;
		}

		private void TypeLoaded(IDebugType type)
		{
			var l = _breakPoints.Where(bp => BreakpointMatchesType(bp, type));
			foreach (var bp in l)
				_session.CreateBreakpointRequest(new DebugLocation(bp.File));
		}

		private static bool BreakpointMatchesType(IBreakPoint bp, IDebugType type)
		{
			return type.SourceFiles.Contains(bp.File);
		}

		private void BreakpointAdded(IBreakPoint breakpoint)
		{
			_breakPoints.Add(breakpoint);

			foreach (var type in _debugTypeProvider.LoadedTypes.Where(t => BreakpointMatchesType(breakpoint, t)))
				_session.CreateBreakpointRequest(new DebugLocation(breakpoint.File));
		}
	}
}
