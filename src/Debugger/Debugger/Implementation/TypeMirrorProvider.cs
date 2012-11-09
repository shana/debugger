using System;
using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Backend;
using TypeLoadEvent = Mono.Debugger.Soft.TypeLoadEvent;

namespace Debugger.Implementation
{
	[Export(typeof(ITypeMirrorProvider))]
	class TypeMirrorProvider : ITypeMirrorProvider
	{
		private readonly IDebuggerSession _session;
		private readonly List<ITypeMirror> _types = new List<ITypeMirror>();

		public event Action<ITypeMirror> TypeLoaded;
		public event Action<ITypeMirror> TypeUnloaded;

		[ImportingConstructor]
		internal TypeMirrorProvider(IDebuggerSession session)
		{
			_session = session;
			//_session.VM.OnTypeLoad += AssemblyUnloaded;
			_session.VM.OnTypeLoad += OnTypeLoaded;
		}

		public ITypeMirror[] LoadedTypesMirror
		{
			get { return _types.ToArray(); }
		}

		private void OnTypeLoaded (ITypeLoadEvent ev)
		{

//			_types.Add (ev.Type);
//			if (TypeLoaded != null)
//				TypeLoaded (ev.Type);
		}

		private void AssemblyUnloaded(IAssemblyMirror assemblyMirror)
		{
			var unloadedTypes = _types.Where(t => t.Assembly == assemblyMirror).ToArray();
			foreach (var type in unloadedTypes)
				OnTypeUnloaded(type);
		}

		private void OnTypeUnloaded(ITypeMirror typeMirror)
		{
			var removed = _types.Remove(typeMirror);
			if (!removed)
				Console.WriteLine("Got Type Unload event for typeMirror that we didnt know about.");

			if (TypeUnloaded != null)
				TypeUnloaded(typeMirror);
		}
	}
}
