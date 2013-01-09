namespace Debugger.Backend
{
	public interface IEventRequest : IWrapper
	{
		void Enable ();
		void Disable ();
	}
}
