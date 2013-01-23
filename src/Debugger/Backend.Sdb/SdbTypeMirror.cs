using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbTypeMirror : Wrapper, ITypeMirror
	{
		private readonly TypeMirror sdbType;
		private IEnumerable<IMethodMirror> methodsMirror;
		private IEnumerable<string> sourceFiles;
		private IAssemblyMirror assembly;
		public IAssemblyMirror Assembly
		{
			get
			{
				if (assembly == null)
					assembly = Cache.Lookup<SdbAssemblyMirror> (sdbType.Assembly);
				return assembly;
			}
		}

		public IEnumerable<string> SourceFiles
		{
			get
			{
				if (sourceFiles == null)
					sourceFiles = this.sdbType.GetSourceFiles (true);
				return sourceFiles;
			}
		}

		public IEnumerable<IMethodMirror> Methods
		{
			get
			{
				if (methodsMirror == null)
					methodsMirror = sdbType.GetMethods ().Select (m => Cache.Lookup<SdbMethodMirror> (m) as IMethodMirror);
				return methodsMirror;
			}
		}

		public string Name
		{
			get { return sdbType.Name; }
		}

		public string FullName
		{
			get { return sdbType.FullName; }
		}

		public SdbTypeMirror (TypeMirror sdbType)
			: base (sdbType)
		{
			this.sdbType = sdbType;
			assembly = Cache.Lookup<SdbAssemblyMirror> (sdbType.Assembly);
		}

		public override string ToString ()
		{
			return FullName;
		}
	}
}
