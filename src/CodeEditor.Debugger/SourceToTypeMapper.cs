using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeEditor.Composition;
using Mono.Debugger.Soft;

/*
namespace CodeEditor.Debugger
{
	[Export(typeof(ISourceToTypeMapper))]
	class SourceToTypeMapper : ISourceToTypeMapper
	{
		private readonly IDebuggerSession _session;
		private readonly IDebugTypeProvider _debugTypeProvider;
		private Dictionary<TypeMirror, List<string>> _typeToSourceFiles = new Dictionary<TypeMirror, List<string>>();

		[ImportingConstructor]
		public SourceToTypeMapper(IDebuggerSession session, IDebugTypeProvider debugTypeProvider)
		{
			_session = session;
			_debugTypeProvider = debugTypeProvider;
			_debugTypeProvider.TypeUnloaded += typeMirror => _typeToSourceFiles.Remove(typeMirror);
		}

		public IEnumerable<TypeMirror> TypesFor(string file)
		{
			return _debugTypeProvider.LoadedTypes.Where(type => SourcesFor(type).Contains(file));
		}

		private IEnumerable<string> SourcesFor(TypeMirror type)
		{
			List<string> files;
			if (_typeToSourceFiles.TryGetValue(type, out files))
				return files;

			files = new List<string>(type.GetSourceFiles(true));
			_typeToSourceFiles[type] = files;

			return files;
		}
	}

	public interface ISourceToTypeMapper
	{
		IEnumerable<TypeMirror> TypesFor(string file);
	}
}*/
