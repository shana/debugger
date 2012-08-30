using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Backend.Sdb
{
	public class SdbAssemblyMirror : IAssemblyMirror
	{
		public SdbAssemblyMirror(AssemblyMirror assemblyMirror)
		{
			Mirror = assemblyMirror;
		}

		public AssemblyMirror Mirror { get; private set; }
	}
}
