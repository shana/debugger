using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CodeEditor.Composition;
using Debugger.Backend;

namespace Debugger.DummyProviders
{
	[Export(typeof(ITypeProvider))]
	public class TypeProvider : ITypeProvider
	{
		private readonly IVirtualMachine virtualMachine;
		private readonly List<ITypeMirror> loadedTypes = new List<ITypeMirror> ();
		private readonly List<string> filter = new List<string> ();
		private readonly List<string> sourceFiles = new List<string> ();
		private string basePath;

		public string BasePath {
			get { return basePath; }
			set
			{
				basePath = value;
				AddFilter (basePath);
			}
		}

		public IList<ITypeMirror> LoadedTypes { get { return loadedTypes.AsReadOnly (); } }
		public IList<string> SourceFiles { get { return sourceFiles.AsReadOnly (); } }

		public event Action<ITypeMirror> TypeLoaded;
		public event Action<ITypeMirror> TypeUnloaded;

		[ImportingConstructor]
		public TypeProvider (IVirtualMachine virtualMachine)
		{
			this.virtualMachine = virtualMachine;
			this.virtualMachine.AssemblyUnloaded += OnAssemblyUnloaded;
			this.virtualMachine.TypeLoaded += OnTypeLoaded;
		}

		private void OnAssemblyUnloaded (IAssemblyEvent assemblyEvent)
		{
			var unloaded = new List<ITypeMirror> ();
			foreach (var loadedType in LoadedTypes)
			{
				if (loadedType.Assembly.Equals (assemblyEvent.Assembly) && !unloaded.Contains (loadedType))
					unloaded.Add (loadedType);
			}

			unloaded.ForEach (t => loadedTypes.Remove (t));
			if (TypeUnloaded != null)
				unloaded.ForEach (t => TypeUnloaded (t));
		}

		private void OnTypeLoaded (ITypeEvent typeEvent)
		{
			if (filter.Count > 0 && !filter.Any (f => typeEvent.Type.Assembly.Path.StartsWith (f)))
				return;

			loadedTypes.Add (typeEvent.Type);
			if (TypeLoaded != null)
				TypeLoaded (typeEvent.Type);
		}

		public void AddFilter (string path)
		{
			if (!filter.Contains (path))
				filter.Add (path);
		}

		public void ClearFilters ()
		{
			filter.Clear ();
		}

		public IList<ITypeMirror> TypesFor (string file)
		{
			if (Path.IsPathRooted (file))
				return loadedTypes.Where (t => t.SourceFiles.Contains (file)).ToList ();
			return loadedTypes.Where (t => t.SourceFiles.Select (s => Path.GetFileName (s)).Contains (file)).ToList ();
		}

		public string MapFullPath (string path)
		{
			if (Path.IsPathRooted (path))
				return path;
			return Path.Combine (basePath, path);
		}
	}
}
