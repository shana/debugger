namespace Debugger.Backend
{
	public interface IBreakpoint : IEventRequest
	{
		ILocation Location { get; }
		bool Enabled { get; }
	}
}
