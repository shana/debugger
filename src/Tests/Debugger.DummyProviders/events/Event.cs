using System;
using Debugger.Backend;
using Debugger.Backend.Event;

namespace Debugger.DummyProviders
{
	class Event : IEvent
	{
		public T Unwrap<T> () where T : class
		{
			throw new NotImplementedException ();
		}

		public bool Cancel { get; set; }
		public State State { get; set; }
		public IThreadMirror Thread { get; set; }
		public IEventRequest Request { get; set; }
	}
}
