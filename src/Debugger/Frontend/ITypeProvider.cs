using System;
using Debugger.Backend;

namespace Debugger
{
	public interface ITypeProvider
	{
		event Action<ITypeEvent, ITypeMirror> TypeLoaded;
		event Action<ITypeMirror> TypeUnloaded;
		ITypeMirror[] LoadedTypes { get; }
		void AddFilter (params string[] typeNames);
		void RemoveFilter (params string[] typeNames);
		void RemoveAllFilters ();
	}
}
