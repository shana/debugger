using System.Collections.Generic;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger
{
	public interface ISourceToTypeMapper
	{
		IEnumerable<IDebugType> TypesFor(string file);
	}
}