using System;
using Debugger.Backend;

namespace Debugger
{
	public interface ITypeMirrorProvider
	{
		event Action<ITypeMirror> TypeLoaded;
		event Action<ITypeMirror> TypeUnloaded;
		ITypeMirror[] LoadedTypesMirror { get; }
	}
}
