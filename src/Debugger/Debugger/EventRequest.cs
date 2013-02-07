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

		public event Action<IEventRequest> RequestEnabled;
		public event Action<IEventRequest> RequestDisabled;

		public virtual void Enable ()
		{
			if (RequestEnabled != null)
				RequestEnabled (this);
		}

		public virtual void Disable ()
		{
			if (RequestDisabled != null)
				RequestDisabled (this);
		}
	}
}
