using System.Collections.Generic;
using Debugger.Backend;

namespace Debugger
{
	public interface ISourceProvider
	{
		IEnumerable<ITypeMirror> TypesFor(string file);
		IEnumerable<string> SourceFiles { get; }
		void AddFilter (params string[] sourceFiles);
		void RemoveFilter (params string[] sourceFiles);
		void RemoveAllFilters ();
		string GetFullPathForFilename (string name);
	}
}
