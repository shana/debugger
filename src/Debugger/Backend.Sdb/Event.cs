using Debugger.Backend.Event;
using MDS=Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class Event : Wrapper, IEvent
	{
		private IThreadMirror threadMirror;
		private SdbThreadMirror sdbThreadMirror;
		private IEventRequest request;
		public bool Cancel { get; set; }
		public State State { get; private set; }

		public IThreadMirror Thread
		{
			get
			{
				if (threadMirror != null)
					return threadMirror;

				if (sdbThreadMirror == null)
					sdbThreadMirror = Cache.Lookup<SdbThreadMirror> (Unwrap<MDS.Event>().Thread);
				return sdbThreadMirror;
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

		public Event (IThreadMirror thread) : base(null)
		{
			threadMirror = thread;
			request = null;
		}

		public Event (MDS.Event ev, State state = State.None) : base(ev)
		{
			State = state;
		}

	}
}
