using System;
using System.Collections.Generic;
using Debugger.Backend;

namespace Debugger
{
	public interface ITypeProvider
	{
		string BasePath { get; set; }
		IList<ITypeMirror> LoadedTypes { get; }
		IList<string> SourceFiles { get; } 

		event Action<ITypeMirror> TypeLoaded;
		event Action<ITypeMirror> TypeUnloaded;

		void AddFilter (string path);
		void ClearFilters ();
		IList<ITypeMirror> TypesFor(string file);
		string MapFullPath (string path);

		string MapRelativePath (string path);
	}
}
