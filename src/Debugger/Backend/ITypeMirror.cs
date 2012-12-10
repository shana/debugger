using System.Collections.Generic;

namespace Debugger.Backend
{
	public interface ITypeMirror : IWrapper
	{
		string Name { get; }
		IAssemblyMirror Assembly { get; }
		IEnumerable<string> SourceFiles { get; }
		IEnumerable<IMethodMirror> Methods { get; }
	}
}
