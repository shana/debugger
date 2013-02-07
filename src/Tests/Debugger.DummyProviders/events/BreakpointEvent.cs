using Debugger.Backend;

namespace Debugger.DummyProviders
{
	class BreakpointEvent : Event, IBreakpointEvent
	{
		public ILocation Location { get; private set; }
		public IMethodMirror Method { get; private set; }

		public BreakpointEvent (ILocation location, IMethodMirror method)
		{
			Location = location;
			Method = method;
		}
	}
}
