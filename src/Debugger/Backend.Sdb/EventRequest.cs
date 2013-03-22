using System;
using System.Threading;
using MDS=Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	internal class EventRequest : Wrapper, IEventRequest
	{
		private MDS.EventRequest sdbRequest { get { return obj as MDS.EventRequest; }}
		public bool Enabled { get; private set; }

		public EventRequest (MDS.EventRequest sdbRequest) : base(sdbRequest) {}

		public event Action<IEventRequest> OnRequestEnabled;
		public event Action<IEventRequest> OnRequestDisabled;
		public virtual void Enable ()
		{
			Enabled = true;
			LogProvider.WithErrorLogging (sdbRequest.Enable);
			if (OnRequestEnabled != null)
				OnRequestEnabled (this);
		}

		public virtual void Disable ()
		{
			Enabled = false;
			LogProvider.WithErrorLogging (sdbRequest.Disable);
			if (OnRequestDisabled != null)
				OnRequestDisabled (this);
		}
	}
}
