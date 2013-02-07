using System;
using Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	internal class SdbBreakpoint : EventRequest, IBreakpoint
	{
		private BreakpointEventRequest sdbRequest { get { return obj as BreakpointEventRequest; } }

		public SdbBreakpoint (BreakpointEventRequest sdbRequest, ILocation location)
			: base (sdbRequest)
		{
			Location = location;
		}

		public ILocation Location { get; private set; }
		public bool Enabled { get { return sdbRequest.Enabled; }}

	}
}
