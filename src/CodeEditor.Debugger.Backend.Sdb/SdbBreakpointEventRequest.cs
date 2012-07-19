using CodeEditor.Debugger.Backend;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	public class SdbBreakpointEventRequest : IBreakpointEventRequest
	{
		private readonly BreakpointEventRequest _sdbRequest;

		public SdbBreakpointEventRequest(BreakpointEventRequest sdbRequest)
		{
			_sdbRequest = sdbRequest;
		}

		public void Enable()
		{
			_sdbRequest.Enable();
		}
	}
}