using System;

namespace Debugger.Backend
{
	public interface IEventRequest : IWrapper
	{
		event Action<IEventRequest> OnRequestEnabled;
		event Action<IEventRequest> OnRequestDisabled;

		void Enable ();
		void Disable ();
	}
}
