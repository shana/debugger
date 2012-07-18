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
		private ISourceToTypeMapper SourceToTypeMapper { get; set; }

		[Import]
		private ILogProvider LogProvider { get; set; }

		public void OnCreate(IDebuggerSession session)
		{
			new BreakpointMediator(session,DebugBreakPointProvider, DebugTypeProvider, SourceToTypeMapper, LogProvider);
		}
	}
	
	class BreakpointMediator
	{
		private readonly IDebuggerSession _session;
		private readonly IDebugBreakPointProvider _debugBreakPointProvider;
		private readonly IDebugTypeProvider _debugTypeProvider;
		private readonly ISourceToTypeMapper _sourceToTypeMapper;
		private readonly ILogProvider _logProvider;
		private readonly List<IBreakPoint> _breakPoints = new List<IBreakPoint>();

		public BreakpointMediator(IDebuggerSession session, IDebugBreakPointProvider debugBreakPointProvider, IDebugTypeProvider debugTypeProvider, ISourceToTypeMapper sourceToTypeMapper, ILogProvider logProvider)
		{
			_session = session;
			_debugBreakPointProvider = debugBreakPointProvider;
			_debugTypeProvider = debugTypeProvider;
			_sourceToTypeMapper = sourceToTypeMapper;
			_logProvider = logProvider;
			_debugBreakPointProvider.BreakpointAdded += BreakpointAdded;
			_debugTypeProvider.TypeLoaded += TypeLoaded;
		}

		private void TypeLoaded(IDebugType type)
		{
			var l = _breakPoints.Where(bp => type.SourceFiles.Contains(bp.File));
			foreach (var bp in l)
				_session.CreateBreakpointRequest(new DebugLocation());
		}

		private void BreakpointAdded(IBreakPoint breakpoint)
		{
			_breakPoints.Add(breakpoint);

			foreach (var type in _debugTypeProvider.LoadedTypes.Where(t => t.SourceFiles.Contains(breakpoint.File)))
				_session.CreateBreakpointRequest(new DebugLocation());
		}


	}
}
