using Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbBreakpointEventRequest : Wrapper, IBreakpointEventRequest
	{
		private readonly BreakpointEventRequest _sdbRequest;

		public SdbBreakpointEventRequest(BreakpointEventRequest sdbRequest) : base(sdbRequest)
		{
			_sdbRequest = sdbRequest;
		}

		public void Enable()
		{
			_sdbRequest.Enable();
		}
	}
}
