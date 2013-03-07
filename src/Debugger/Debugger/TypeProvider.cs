using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Backend;

namespace Debugger
{
	[Export (typeof (ITypeProvider))]
	public class TypeProvider : ITypeProvider
	{
		private readonly IVirtualMachine virtualMachine;
		private readonly List<ITypeMirror> loadedTypes = new List<ITypeMirror> ();
		private readonly List<string> filter = new List<string> ();
		private string basePath = String.Empty;
		private IList<string> sourceFiles = new List<string> ();

		public string BasePath
		{
			get { return basePath; }
			set
			{
				basePath = NormalizePath(value);
				AddFilter (basePath);
			}
		}

		public IList<ITypeMirror> LoadedTypes { get { return loadedTypes.ToArray (); } }
		public IList<string> SourceFiles { get { return sourceFiles.ToArray (); } }

		public event Action<ITypeMirror> TypeLoaded;
		public event Action<ITypeMirror> TypeUnloaded;

		[ImportingConstructor]
		public TypeProvider (IVirtualMachine virtualMachine)
		{
			this.virtualMachine = virtualMachine;
			this.virtualMachine.AssemblyLoaded += OnAssemblyLoaded;
			this.virtualMachine.AssemblyUnloaded += OnAssemblyUnloaded;
			this.virtualMachine.TypeLoaded += OnTypeLoaded;
		}

		private void OnAssemblyLoaded (IAssemblyEvent ev)
		{
			var path = NormalizePath (ev.Assembly.Path);
			//ev.Cancel = true;
			ev.Cancel = filter.Count > 0 && !filter.Any (f => path.StartsWith (f));
		}

		private void OnAssemblyUnloaded (IAssemblyEvent assemblyEvent)
		{
			var unloaded = new List<ITypeMirror> ();
			foreach (var loadedType in loadedTypes)
			{
				if (loadedType.Assembly.Equals (assemblyEvent.Assembly) && !unloaded.Contains (loadedType))
					unloaded.Add (loadedType);
			}

			foreach (var typeMirror in unloaded)
				foreach (var file in typeMirror.SourceFiles)
					if (sourceFiles.Contains (file))
						sourceFiles.Remove (file);

			unloaded.ForEach (t => loadedTypes.Remove (t));
			if (TypeUnloaded != null)
				unloaded.ForEach (t => TypeUnloaded (t));
		}

		private void OnTypeLoaded (ITypeEvent typeEvent)
		{
			//LogProvider.Log ("TypeLoaded {0} {1}", typeEvent.Type.Name, typeEvent.Type.Assembly.Path);
			if (!filter.Any (f => typeEvent.Type.Assembly.Path.StartsWith (f)))
				return;

			loadedTypes.Add (typeEvent.Type);
			foreach (var file in typeEvent.Type.SourceFiles)
				if (!sourceFiles.Contains (file))
					sourceFiles.Add (file);
			if (TypeLoaded != null)
				TypeLoaded (typeEvent.Type);
		}

		public void AddFilter (string path)
		{
			if (String.IsNullOrEmpty(path))
				return;
			path = NormalizePath (path);
			if (!filter.Contains (path))
				filter.Add (path);
		}

		public void ClearFilters ()
		{
			filter.Clear ();
		}


		public IList<ITypeMirror> TypesFor (string file)
		{
			file = MapFullPath (file);
			return loadedTypes.Where (t => t.SourceFiles.Contains (file)).ToList ();
		}

		public string MapFullPath (string path)
		{
			path = NormalizePath (path);
			if (Path.IsPathRooted (path))
				return path;
			return Path.Combine (basePath, Path.Combine("Assets", path));
		}

		public string MapRelativePath (string path)
		{
			path = NormalizePath (path);
			if (!Path.IsPathRooted (path))
				return path;
			var assets = Path.Combine (BasePath, "Assets" + Path.DirectorySeparatorChar);
			if (path.StartsWith (assets))
				return path.Substring (assets.Length);
			return path;
		}

		private static string NormalizePath (string path)
		{
			path = path.Replace ('/', Path.DirectorySeparatorChar);
			path = path.Replace ('\\', Path.DirectorySeparatorChar);
			return path;
		}
	}
}
