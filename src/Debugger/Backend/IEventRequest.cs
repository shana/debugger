using System;

namespace Debugger.Backend
{
	public interface IEventRequest : IWrapper
	{
		event Action<IEventRequest> RequestEnabled;
		event Action<IEventRequest> RequestDisabled;

		void Enable ();
		void Disable ();
	}
}
