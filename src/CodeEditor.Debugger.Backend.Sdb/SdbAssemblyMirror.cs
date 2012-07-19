using CodeEditor.Debugger.Backend;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
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
