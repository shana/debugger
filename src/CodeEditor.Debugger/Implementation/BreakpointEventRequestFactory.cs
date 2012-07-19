using CodeEditor.Composition;

namespace CodeEditor.Debugger.Implementation
{
	[Export(typeof(IBreakpointEventRequestFactory))]
	class BreakpointEventRequestFactory : IDebuggerSessionCreationListener, IBreakpointEventRequestFactory
	{
		private IDebuggerSession _session;

		public void OnCreate(IDebuggerSession session)
		{
			_session = session;
		}

		public IDebugBreakpointEventRequest Create(IDebugLocation location)
		{
			var debugLocation = (DebugLocation) location;
			var request = _session.CreateBreakpointRequest(debugLocation.MDSLocation);
			return new DebugBreakpointEventRequest(request);
		}
	}
}
