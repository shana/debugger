namespace CodeEditor.Debugger.Implementation
{
	class DebugType : IDebugType
	{
		public IDebugAssembly Assembly { get; private set; }

		public DebugType(IDebugAssembly assembly)
		{
			Assembly = assembly;
		}
	}
}
