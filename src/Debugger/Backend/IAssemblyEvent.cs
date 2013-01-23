namespace Debugger.Backend
{
	public interface IAssemblyEvent : IEvent
	{
		IAssemblyMirror Assembly { get; }
	}
}
