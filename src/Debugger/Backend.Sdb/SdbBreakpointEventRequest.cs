using Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbBreakpointEventRequest : Wrapper, IBreakpointEventRequest
	{
		private BreakpointEventRequest _sdbRequest { get { return _obj as BreakpointEventRequest; }}

		public SdbBreakpointEventRequest(BreakpointEventRequest sdbRequest) : base(sdbRequest) {}

		public void Enable()
		{
			_sdbRequest.Enable();
		}

		public void Disable ()
		{
			_sdbRequest.Disable ();
		}
	}
}
