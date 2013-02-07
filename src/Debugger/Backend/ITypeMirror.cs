using System.Collections.Generic;
using Mono.Cecil;

namespace Debugger.Backend
{
	public interface ITypeMirror : IWrapper
	{
		string Name { get; }
		IAssemblyMirror Assembly { get; }
		IList<string> SourceFiles { get; }
		IList<IMethodMirror> Methods { get; }
		string FullName { get; }
	}
}
