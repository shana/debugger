using System;
using Debugger.Backend;

namespace Debugger.DummyProviders
{
	class EventRequest : BaseMirror, IEventRequest
	{
		public event Action<IEventRequest> OnRequestEnabled;
		public event Action<IEventRequest> OnRequestDisabled;

		public virtual void Enable ()
		{
			if (OnRequestEnabled != null)
				OnRequestEnabled (this);
		}

		public virtual void Disable ()
		{
			if (OnRequestDisabled != null)
				OnRequestDisabled (this);
		}
	}
}
