using Debugger.Backend;

namespace Debugger.DummyProviders
{
	class AssemblyEvent : Event, IAssemblyEvent
	{
		public IAssemblyMirror Assembly { get; private set; }

		public AssemblyEvent (IAssemblyMirror assembly)
		{
			Assembly = assembly;
		}
	}
}
