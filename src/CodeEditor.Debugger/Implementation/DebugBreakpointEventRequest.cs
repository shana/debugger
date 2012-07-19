using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	class DebugBreakpointEventRequest : IDebugBreakpointEventRequest
	{
		private readonly BreakpointEventRequest _mdsRequest;

		public DebugBreakpointEventRequest(BreakpointEventRequest mdsRequest)
		{
			_mdsRequest = mdsRequest;
		}

		public void Enable()
		{
			_mdsRequest.Enable();
		}
	}
}