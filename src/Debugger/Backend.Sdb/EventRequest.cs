using MDS=Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	internal class EventRequest : Wrapper, IEventRequest
	{
		private MDS.EventRequest _sdbRequest { get { return _obj as MDS.EventRequest; }}

		public EventRequest (MDS.EventRequest sdbRequest) : base(sdbRequest) {}

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
