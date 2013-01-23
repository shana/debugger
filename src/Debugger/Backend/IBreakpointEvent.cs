namespace Debugger.Backend
{
	public interface IBreakpointEvent : IEvent
	{
		ILocation Location { get; }
		IMethodMirror Method { get; }
	}
}
