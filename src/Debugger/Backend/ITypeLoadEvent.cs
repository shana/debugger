namespace Debugger.Backend
{
	public interface ITypeLoadEvent : IEvent
	{
		ITypeMirror Type { get; }
	}
}
