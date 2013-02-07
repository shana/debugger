using Debugger.Backend.Event;
using MDS = Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class TypeEvent : Event, ITypeEvent
	{
		private SdbTypeMirror typeMirror;

		public TypeEvent (MDS.Event ev)
			: base (ev)
		{
		}

		public ITypeMirror Type
		{
			get
			{
				if (typeMirror == null)
					typeMirror = Cache.Lookup<SdbTypeMirror> (Unwrap<MDS.TypeLoadEvent> ().Type);
				return typeMirror;
			}
		}
	}
}
