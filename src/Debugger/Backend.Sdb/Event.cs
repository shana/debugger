using MDS=Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class Event : Wrapper, IEvent
	{
		private SdbThreadMirror _threadMirror;

		public Event (MDS.Event ev) : base(ev)
		{
		}

		public IThreadMirror Thread
		{
			get
			{
				if (_threadMirror == null)
					_threadMirror = new SdbThreadMirror (Unwrap<MDS.Event>().Thread);
				return _threadMirror;
			}
		}
	}
}
