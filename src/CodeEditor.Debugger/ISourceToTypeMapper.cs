using System.Collections.Generic;
using CodeEditor.Debugger.Backend;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger
{
	public interface ISourceToTypeMapper
	{
		IEnumerable<ITypeMirror> TypesFor(string file);
	}
}