using CodeEditor.Debugger.Backend;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	public class SdbBreakpointEventRequest : IBreakpointEventRequest
	{
		private readonly BreakpointEventRequest _mdsRequest;

		public SdbBreakpointEventRequest(BreakpointEventRequest mdsRequest)
		{
			_mdsRequest = mdsRequest;
		}

		public void Enable()
		{
			_mdsRequest.Enable();
		}
	}
}