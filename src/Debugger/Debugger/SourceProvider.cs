using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Backend;

namespace Debugger
{
	[Export (typeof (ISourceProvider))]
	class SourceProvider : ISourceProvider
	{
		private readonly ITypeProvider typeProvider;
		private readonly List<string> filter = new List<string> ();
		private readonly Dictionary<string, List<ITypeMirror>> sourceToTypes = new Dictionary<string, List<ITypeMirror>> ();

		public IEnumerable<string> SourceFiles { get { return sourceToTypes.Keys.Distinct (); } }

		[ImportingConstructor]
		public SourceProvider (ITypeProvider typeProvider)
		{
			this.typeProvider = typeProvider;
			this.typeProvider.TypeLoaded += TypeLoaded;
			this.typeProvider.TypeUnloaded += TypeUnloaded;
		}

		private void TypeLoaded (ITypeEvent ev, ITypeMirror type)
		{
			if (ev.Cancel)
				return;

			var sourceFiles = type.SourceFiles.ToArray ();
			if (filter.Count > 0)
				sourceFiles = sourceFiles.Where (x => filter.Contains (Path.GetFileName (x))).ToArray ();
			if (sourceFiles.Length == 0)
			{
				ev.Cancel = true;
				return;
			}

			foreach (var file in sourceFiles)
				AddTypeToSourceToTypes (type, file);
		}

		private void TypeUnloaded (ITypeMirror typeMirror)
		{
			var map = sourceToTypes.Where (x => x.Value.Contains (typeMirror)).ToArray ();
			foreach (var kvp in map)
				sourceToTypes.Remove (kvp.Key);
		}

		private void AddTypeToSourceToTypes (ITypeMirror typeMirror, string file)
		{
			if (!sourceToTypes.ContainsKey (file))
				sourceToTypes.Add (file, new List<ITypeMirror> ());

			sourceToTypes[file].Add (typeMirror);
		}

		public string GetFullPathForFilename (string name)
		{
			return SourceFiles.FirstOrDefault (f => Path.GetFileName (f) == name);
		}

		public IEnumerable<ITypeMirror> TypesFor (string file)
		{
			if (sourceToTypes.ContainsKey (file))
				return sourceToTypes[file];

			return sourceToTypes.Where (k => Path.GetFileName (k.Key) == file).SelectMany (t => t.Value);
		}

		public void AddFilter (params string[] sourceFiles)
		{
			foreach (var file in sourceFiles)
				if (!filter.Contains (file))
					filter.Add (file);
		}

		public void RemoveFilter (params string[] sourceFiles)
		{
			foreach (var file in sourceFiles)
				if (filter.Contains (file))
					filter.Remove (file);
		}

		public void RemoveAllFilters ()
		{
			filter.Clear ();
		}
	}
}
