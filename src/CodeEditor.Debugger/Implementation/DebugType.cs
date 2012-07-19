using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	class DebugType : IDebugType
	{
		private readonly TypeMirror _type;
		private IDebugMethod[] _methods;
		private string[] _sourceFiles;
		public IDebugAssembly Assembly { get; private set; }

		public string[] SourceFiles
		{
			get
			{
				if (_sourceFiles != null)
					return _sourceFiles;
				_sourceFiles = _type.GetSourceFiles(true);
				return _sourceFiles;
			}
		}

		public IEnumerable<IDebugMethod> Methods
		{
			get
			{
				if (_methods != null)
					return _methods;
				_methods = _type.GetMethods().Select(DebugMethodFor).ToArray();
				return _methods;
			}
		}

		public IDebugMethod DebugMethodFor(MethodMirror methodMirror)
		{
			return new DebugMethod(methodMirror);
		}

		public DebugType(TypeMirror type, IDebugAssembly assembly)
		{
			_type = type;
			Assembly = assembly;
		}
	}
}
