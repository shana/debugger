using Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbAssemblyMirror : Wrapper, IAssemblyMirror
	{
		public SdbAssemblyMirror(AssemblyMirror assemblyMirror) : base (assemblyMirror)
		{
			Mirror = assemblyMirror;
		}

		public AssemblyMirror Mirror { get; private set; }
	}
}
