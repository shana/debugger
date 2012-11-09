using MDS = Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class TypeLoadEvent : Event, ITypeLoadEvent
	{
		private SdbTypeMirror _typeMirror;

		public TypeLoadEvent(MDS.Event ev) : base(ev)
		{
		}

		public ITypeMirror Type
		{
			get
			{
				if (_typeMirror == null)
					_typeMirror = new SdbTypeMirror(Unwrap<MDS.TypeLoadEvent>().Type, new SdbAssemblyMirror(Unwrap<MDS.TypeLoadEvent>().Type.Assembly));
				return _typeMirror;
			}
		}
	}
}
