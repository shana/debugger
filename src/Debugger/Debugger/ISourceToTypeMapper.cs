using System.Collections.Generic;
using Debugger.Backend;

namespace Debugger
{
	public interface ISourceToTypeMapper
	{
		IEnumerable<ITypeMirror> TypesFor(string file);
	}
}
