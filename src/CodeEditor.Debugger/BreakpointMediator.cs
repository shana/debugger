using System;
using System.Collections.Generic;
using CodeEditor.Composition;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger
{
	[Export(typeof(IDebuggerSessionCreationListener))]
	class BreakPointMediatorFactory : IDebuggerSessionCreationListener
	{
		[Import]
		private IDebugBreakPointProvider DebugBreakPointProvider { get; set; }

		public void OnCreate(IDebuggerSession session)
		{
			new BreakpointMediator(session,DebugBreakPointProvider);
		}
	}
	
	class BreakpointMediator
	{
		private readonly IDebuggerSession _session;
		private readonly IDebugBreakPointProvider _debugBreakPointProvider;

		public BreakpointMediator(IDebuggerSession session, IDebugBreakPointProvider debugBreakPointProvider)
		{
			_session = session;
			_debugBreakPointProvider = debugBreakPointProvider;
			_debugBreakPointProvider.BreakPointAdded += BreakPointAdded;
		}

		private void BreakPointAdded(IBreakPoint breakpoint)
		{
		}
	}
}
