using Debugger.Backend.Event;
using MDS = Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class AssemblyEvent : Event, IAssemblyEvent
	{
		private SdbAssemblyMirror mirror;
		public IAssemblyMirror Assembly
		{
			get
			{
				if (mirror == null) {
					if (obj is MDS.AssemblyLoadEvent)
						mirror = Cache.Lookup<SdbAssemblyMirror> (Unwrap<MDS.AssemblyLoadEvent> ().Assembly);
					else
						mirror = Cache.Lookup<SdbAssemblyMirror> (Unwrap<MDS.AssemblyUnloadEvent> ().Assembly);
				}
				return mirror;
			}
		}

		public AssemblyEvent (MDS.Event ev, State state)
			: base (ev, state)
		{ }
	}
}
