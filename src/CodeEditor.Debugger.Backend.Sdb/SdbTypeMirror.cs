using System.Collections.Generic;
using System.Linq;
using CodeEditor.Debugger.Backend;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	public class SdbTypeMirror : ITypeMirror
	{
		private readonly TypeMirror _sdbType;
		private IMethodMirror[] _methodsMirror;
		private string[] _sourceFiles;
		public IAssemblyMirror AssemblyMirror { get; private set; }

		public string[] SourceFiles
		{
			get
			{
				if (_sourceFiles != null)
					return _sourceFiles;
				_sourceFiles = _sdbType.GetSourceFiles(true);
				return _sourceFiles;
			}
		}

		public IEnumerable<IMethodMirror> Methods
		{
			get
			{
				if (_methodsMirror != null)
					return _methodsMirror;
				_methodsMirror = _sdbType.GetMethods().Select(DebugMethodFor).ToArray();
				return _methodsMirror;
			}
		}

		public IMethodMirror DebugMethodFor(MethodMirror methodMirror)
		{
			return new SdbMethodMirror(methodMirror);
		}

		public SdbTypeMirror(TypeMirror sdbType, IAssemblyMirror assemblyMirror)
		{
			_sdbType = sdbType;
			AssemblyMirror = assemblyMirror;
		}
	}
}
