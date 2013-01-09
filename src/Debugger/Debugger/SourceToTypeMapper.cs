using System.Collections.Generic;
using CodeEditor.Composition;
using Debugger.Backend;

namespace Debugger
{
	[Export(typeof(ISourceToTypeMapper))]
	class SourceToTypeMapper : ISourceToTypeMapper
	{
		private readonly ITypeMirrorProvider _typeMirrorProvider;

		private readonly Dictionary<string, List<ITypeMirror>> _sourceToTypes = new Dictionary<string, List<ITypeMirror>>();

		[ImportingConstructor]
		public SourceToTypeMapper(ITypeMirrorProvider typeMirrorProvider)
		{
			_typeMirrorProvider = typeMirrorProvider;
			_typeMirrorProvider.TypeLoaded += TypeMirrorLoaded;
		}

		private void TypeMirrorLoaded(ITypeMirror typeMirror)
		{
			var sourceFiles = typeMirror.SourceFiles;
			foreach (var file in sourceFiles)
				AddTypeToSourceToTypes(typeMirror, file);
		}

		private void AddTypeToSourceToTypes(ITypeMirror typeMirror, string file)
		{
			if (!_sourceToTypes.ContainsKey(file))
				_sourceToTypes.Add(file, new List<ITypeMirror>());

			_sourceToTypes[file].Add(typeMirror);
		}

		public IEnumerable<ITypeMirror> TypesFor(string file)
		{
			if (!_sourceToTypes.ContainsKey(file))
				return new ITypeMirror[0];

			return _sourceToTypes[file];
		}
	}
}
