namespace Debugger.Backend
{
	public interface ITypeEvent : IEvent
	{
		ITypeMirror Type { get; }
	}
}
