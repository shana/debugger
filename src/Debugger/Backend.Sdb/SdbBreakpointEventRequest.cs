using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Backend.Sdb
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