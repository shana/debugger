using System;
using Debugger.Backend;

namespace Debugger
{
	public class EventRequest : IEventRequest
	{
		public T Unwrap<T> () where T : class
		{
			throw new NotImplementedException ();
		}

		public event Action<IEventRequest> OnRequestEnabled;
		public event Action<IEventRequest> OnRequestDisabled;

		public bool Enabled { get; private set; }

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
