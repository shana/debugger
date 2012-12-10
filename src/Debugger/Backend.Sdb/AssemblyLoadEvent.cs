using MDS = Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class AssemblyLoadEvent : Event, IAssemblyLoadEvent
	{
		public AssemblyLoadEvent (MDS.Event ev)
			: base (ev)
		{ }
	}
}
