using System.Collections.Generic;

namespace CodeEditor.Debugger
{
	public interface IDebugType
	{
		IDebugAssembly Assembly { get; }
		string[] SourceFiles { get; }
		IEnumerable<IDebugMethod> Methods { get; }
	}
}