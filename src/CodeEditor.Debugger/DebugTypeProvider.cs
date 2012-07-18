using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeEditor.Composition;

namespace CodeEditor.Debugger
{
	[Export(typeof(IDebugTypeProvider))]
	class DebugTypeProvider : IDebugTypeProvider
	{
		private readonly IDebuggerSession _session;
		private readonly List<IDebugType> _types = new List<IDebugType>();

		public event Action<IDebugType> TypeLoaded;
		public event Action<IDebugType> TypeUnloaded;

		[ImportingConstructor]
		internal DebugTypeProvider(IDebuggerSession session)
		{
			_session = session;
			_session.AssemblyUnloaded += AssemblyUnloaded;
			_session.TypeLoaded += OnTypeLoaded;
		}

		public IDebugType[] LoadedTypes
		{
			get { return _types.ToArray(); }
		}

		private void OnTypeLoaded(IDebugType type)
		{
			_types.Add(type);
			if (TypeLoaded != null)
				TypeLoaded(type);
		}

		private void AssemblyUnloaded(IDebugAssembly debugAssembly)
		{
			var unloadedTypes = _types.Where(t => t.Assembly == debugAssembly).ToArray();
			foreach (var type in unloadedTypes)
				OnTypeUnloaded(type);
		}

		private void OnTypeUnloaded(IDebugType type)
		{
			var removed = _types.Remove(type);
			if (!removed)
				throw new InvalidDataException("Got Type Unloade event for type that we didnt know about.");

			if (TypeUnloaded != null)
				TypeUnloaded(type);
		}
	}

	public interface IDebugTypeProvider
	{
		event Action<IDebugType> TypeLoaded;
		event Action<IDebugType> TypeUnloaded;
		IDebugType[] LoadedTypes { get; }
	}
}
