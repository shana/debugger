using Debugger.Backend.Event;
using MDS=Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class Event : Wrapper, IEvent
	{
		private SdbThreadMirror threadMirror;
		private IEventRequest request;
		public bool Cancel { get; set; }
		public State State { get; private set; }

		public IThreadMirror Thread
		{
			get
			{
				if (threadMirror == null)
					threadMirror = Cache.Lookup<SdbThreadMirror> (Unwrap<MDS.Event>().Thread);
				return threadMirror;
			}
		}

		public IEventRequest Request
		{
			get
			{
				if (request == null)
					request = Cache.Lookup<EventRequest> (Unwrap<MDS.Event> ().Request);
				return request;
			}
		}

		public Event (MDS.Event ev, State state = State.None) : base(ev)
		{
			State = state;
		}

	}
}
