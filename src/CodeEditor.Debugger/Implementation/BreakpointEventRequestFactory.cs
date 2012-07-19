using CodeEditor.Composition;
using CodeEditor.Debugger.Backend;
using CodeEditor.Debugger.Backend.Sdb;

namespace CodeEditor.Debugger.Implementation
{
	[Export(typeof(IBreakpointEventRequestFactory))]
	[Export(typeof(IDebuggerSessionCreationListener))]
	class BreakpointEventRequestFactory : IDebuggerSessionCreationListener, IBreakpointEventRequestFactory
	{
		private IDebuggerSession _session;

		public void OnCreate(IDebuggerSession session)
		{
			_session = session;
		}

		public IBreakpointEventRequest Create(ILocation location)
		{
			var sdbLocation = (SdbLocation) location;
			var request = _session.CreateBreakpointRequest(sdbLocation.MDSLocation);
			return new SdbBreakpointEventRequest(request);
		}
	}
}
