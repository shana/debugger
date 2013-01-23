using System;
using System.Threading;
using MDS=Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	internal class EventRequest : Wrapper, IEventRequest
	{
		private MDS.EventRequest sdbRequest { get { return obj as MDS.EventRequest; }}

		public EventRequest (MDS.EventRequest sdbRequest) : base(sdbRequest) {}

		public void Enable()
		{
			LogProvider.WithErrorLogging (sdbRequest.Enable);
		}

		public void Disable ()
		{
			LogProvider.WithErrorLogging (sdbRequest.Disable);
		}
	}
}
