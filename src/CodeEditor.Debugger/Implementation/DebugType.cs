using System.Collections.Generic;

namespace CodeEditor.Debugger.Implementation
{
	class DebugType : IDebugType
	{
		public IDebugAssembly Assembly { get; private set; }

		public IEnumerable<string> SourceFiles
		{
			get { throw new System.NotImplementedException(); }
		}

		public DebugType(IDebugAssembly assembly)
		{
			Assembly = assembly;
		}
	}
}
