using System.Collections.Generic;

namespace CodeEditor.Debugger.Backend
{
	public interface ITypeMirror
	{
		IAssemblyMirror AssemblyMirror { get; }
		string[] SourceFiles { get; }
		IEnumerable<IMethodMirror> Methods { get; }
	}
}