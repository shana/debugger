using System;
using System.Threading;
using MDS=Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	internal class EventRequest : Wrapper, IEventRequest
	{
		private MDS.EventRequest sdbRequest { get { return obj as MDS.EventRequest; }}

		public EventRequest (MDS.EventRequest sdbRequest) : base(sdbRequest) {}

		public event Action<IEventRequest> RequestEnabled;
		public event Action<IEventRequest> RequestDisabled;
		public virtual void Enable ()
		{
			LogProvider.WithErrorLogging (sdbRequest.Enable);
			if (RequestEnabled != null)
				RequestEnabled (this);

		}

		public virtual void Disable ()
		{
			LogProvider.WithErrorLogging (sdbRequest.Disable);
			if (RequestDisabled != null)
				RequestDisabled (this);
		}
	}
}
