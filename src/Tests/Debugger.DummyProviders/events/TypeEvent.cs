using Debugger.Backend;

namespace Debugger.DummyProviders
{
	class TypeEvent : Event, ITypeEvent
	{
		public ITypeMirror Type { get; private set; }

		public TypeEvent (ITypeMirror type)
		{
			Type = type;
		}
	}
}
