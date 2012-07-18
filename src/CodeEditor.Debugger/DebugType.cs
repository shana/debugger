namespace CodeEditor.Debugger
{
	public interface IDebugType
	{
		IDebugAssembly Assembly { get; }
	}

	class DebugType : IDebugType
	{
		public IDebugAssembly Assembly { get; private set; }

		public DebugType(IDebugAssembly assembly)
		{
			Assembly = assembly;
		}
	}
}
