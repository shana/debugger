namespace Debugger.Backend
{
	namespace Event {
		public enum State
		{
			None,
			Start,
			Stop,
			Disconnect,
			Load,
			Unload,
			Suspend
		}
	}

	public interface IEvent : IWrapper
	{
		bool Cancel { get; set; }
		Event.State State { get; }
		IThreadMirror Thread { get; }
		IEventRequest Request { get; }
	}
}
