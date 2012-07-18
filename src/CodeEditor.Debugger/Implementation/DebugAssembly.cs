using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	class DebugAssembly : IDebugAssembly
	{
		public DebugAssembly(AssemblyMirror assemblyMirror)
		{
			Mirror = assemblyMirror;
		}

		public AssemblyMirror Mirror { get; private set; }
	}
}
