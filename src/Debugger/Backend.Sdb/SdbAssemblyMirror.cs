using Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbAssemblyMirror : Wrapper, IAssemblyMirror
	{
		public AssemblyMirror Mirror { get { return _obj as AssemblyMirror; }}

		public SdbAssemblyMirror(AssemblyMirror assemblyMirror) : base (assemblyMirror) {}
	}
}
