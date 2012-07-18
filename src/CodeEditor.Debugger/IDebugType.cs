using System.Collections.Generic;

namespace CodeEditor.Debugger
{
	public interface IDebugType
	{
		IDebugAssembly Assembly { get; }
		IEnumerable<string> SourceFiles { get; }
		IEnumerable<IDebugMethod> Methods { get; }
	}
}