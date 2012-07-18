using System;

namespace CodeEditor.Debugger
{
	public interface IDebugTypeProvider
	{
		event Action<IDebugType> TypeLoaded;
		event Action<IDebugType> TypeUnloaded;
		IDebugType[] LoadedTypes { get; }
	}
}