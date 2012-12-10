using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbTypeMirror : Wrapper, ITypeMirror
	{
		private readonly TypeMirror _sdbType;
		private readonly List<IMethodMirror> _methodsMirror;
		private readonly List<string> _sourceFiles;
		public IAssemblyMirror Assembly { get; private set; }

		public IEnumerable<string> SourceFiles
		{
			get { return _sourceFiles; }
		}

		public IEnumerable<IMethodMirror> Methods
		{
			get { return _methodsMirror; }
		}

		private List<IMethodMirror> GetMethodMirrors()
		{
				return _sdbType.GetMethods().Select(m => new SdbMethodMirror(m) as IMethodMirror).ToList();
		}

		public SdbTypeMirror(TypeMirror sdbType, IAssemblyMirror assemblyMirror) : base(sdbType)
		{
			_sdbType = sdbType;
			Assembly = assemblyMirror;

			_methodsMirror = GetMethodMirrors();
			_sourceFiles = new List<string>(_sdbType.GetSourceFiles(true));
		}

		public string Name
		{
			get { return _sdbType.Name; }
		}
	}
}
