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
		
		//[Import]
		//private ISourceToTypeMapper SourceToTypeMapper { get; set; }

		//[Import]
		private ILogProvider LogProvider { get; set; }

		public void OnCreate(IDebuggerSession session)
		{

			//new BreakpointMediator(session,DebugBreakPointProvider, DebugTypeProvider, SourceToTypeMapper, LogProvider);
		}
	}
	
	class BreakpointMediator
	{
		private readonly IDebuggerSession _session;
		private readonly IDebugBreakPointProvider _debugBreakPointProvider;
		private readonly IDebugTypeProvider _debugTypeProvider;
		//private readonly ISourceToTypeMapper _sourceToTypeMapper;
		private readonly ILogProvider _logProvider;

		public BreakpointMediator(IDebuggerSession session, IDebugBreakPointProvider debugBreakPointProvider, IDebugTypeProvider debugTypeProvider, /*ISourceToTypeMapper sourceToTypeMapper,*/ ILogProvider logProvider)
		{
			_session = session;
			_debugBreakPointProvider = debugBreakPointProvider;
			_debugTypeProvider = debugTypeProvider;
			//_sourceToTypeMapper = sourceToTypeMapper;
			_logProvider = logProvider;
			_debugBreakPointProvider.BreakPointAdded += BreakPointAdded;
		}

		private void BreakPointAdded(IBreakPoint breakpoint)
		{
			/*
			var types = _sourceToTypeMapper.TypesFor(breakpoint.File);

			var sb = new StringBuilder("Types for " + breakpoint.File + " are ");
			foreach (var type in types)
				sb.Append(type.Name + " ");
			_logProvider.Log(sb.ToString());*/
		}
	}
}
