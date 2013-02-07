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
		private string basePath;

		public string BasePath
		{
			get { return basePath; }
			set
			{
				basePath = value;
				AddFilter (basePath);
			}
		}

		public IList<ITypeMirror> LoadedTypes { get { return loadedTypes.ToArray (); } }
		public IList<string> SourceFiles { get { return loadedTypes.SelectMany (t => t.SourceFiles).ToList ().ToArray (); } }

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
			LogProvider.Log (ev.Assembly.Path);
			ev.Cancel = filter.Count > 0 && !filter.Any (f => ev.Assembly.Path.StartsWith (f));
		}

		private void OnAssemblyUnloaded (IAssemblyEvent assemblyEvent)
		{
			var unloaded = new List<ITypeMirror> ();
			foreach (var loadedType in loadedTypes)
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
			loadedTypes.Add (typeEvent.Type);
			if (TypeLoaded != null)
				TypeLoaded (typeEvent.Type);
		}

		public void AddFilter (string path)
		{
			if (path == null)
				return;
			if (!filter.Contains (path))
				filter.Add (path);
		}

		public void ClearFilters ()
		{
			filter.Clear ();
		}

		public IList<ITypeMirror> TypesFor (string file)
		{
			file = MapFile (file);
			return loadedTypes.Where (t => t.SourceFiles.Contains (file)).ToList ();
		}

		public string MapFile (string file)
		{
			if (Path.IsPathRooted (file))
				return file;
			return Path.Combine (basePath, "Assets/" + file);
		}
	}
}
