using System.Collections.Generic;

namespace CodeEditor.Debugger.Backend
{
	public interface ITypeMirror
	{
		IAssemblyMirror Assembly { get; }
		string[] SourceFiles { get; }
		IMethodMirror[] Methods { get; }
	}
}