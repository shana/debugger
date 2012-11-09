namespace Debugger.Backend
{
	public interface IEvent : IWrapper
	{
		IThreadMirror Thread { get; }
	}
}
