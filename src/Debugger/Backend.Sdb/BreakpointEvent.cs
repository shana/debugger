using System;
using System.Linq;
using Debugger.Backend.Event;
using MDS = Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class BreakpointEvent : Event, IBreakpointEvent
	{
		SdbMethodMirror sdbMethodMirror;
		public ILocation Location {
			get {
				var request = Cache.Lookup<SdbBreakpoint> (Unwrap<MDS.BreakpointEvent> ().Request);
				if (request != null)
					return request.Location;
				return new SdbLocation (Unwrap<MDS.BreakpointEvent> ().Method.SourceFile, (int)Unwrap<MDS.BreakpointEvent> ().Location);
			}
		}

		public IMethodMirror Method
		{
			get
			{
				if (sdbMethodMirror == null)
					sdbMethodMirror = Cache.Lookup<SdbMethodMirror> (Unwrap<MDS.BreakpointEvent> ().Method);
				return sdbMethodMirror;
			}
		}
		public BreakpointEvent (MDS.Event ev)
			: base (ev)
		{
		}
	}
}
