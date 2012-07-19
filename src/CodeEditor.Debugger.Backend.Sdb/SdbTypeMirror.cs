using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Debugger.Backend;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	public class SdbTypeMirror : ITypeMirror
	{
		private readonly TypeMirror _sdbType;
		private readonly Lazy<IMethodMirror[]> _methodsMirror;
		private readonly Lazy<string[]> _sourceFiles;
		public IAssemblyMirror Assembly { get; private set; }

		public string[] SourceFiles
		{
			get { return _sourceFiles.Value; }
		}

		public IMethodMirror[] Methods
		{
			get { return _methodsMirror.Value; }
		}

		private SdbMethodMirror[] GetMethodMirrors()
		{
				return _sdbType.GetMethods().Select(m => new SdbMethodMirror(m)).ToArray();
		}

		public SdbTypeMirror(TypeMirror sdbType, IAssemblyMirror assemblyMirror)
		{
			_sdbType = sdbType;
			Assembly = assemblyMirror;

			_methodsMirror = new Lazy<IMethodMirror[]>(GetMethodMirrors);
			_sourceFiles = new Lazy<string[]>(() => _sdbType.GetSourceFiles(true));
		}
	}
}
