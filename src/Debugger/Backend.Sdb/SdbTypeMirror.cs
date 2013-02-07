using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbTypeMirror : Wrapper, ITypeMirror
	{
		private readonly TypeMirror sdbType;
		private IList<IMethodMirror> methodsMirror;
		private IList<string> sourceFiles;
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

		public IList<string> SourceFiles
		{
			get
			{
				if (sourceFiles == null)
					sourceFiles = Array.AsReadOnly (sdbType.GetSourceFiles (true));
				return sourceFiles;
			}
		}

		public IList<IMethodMirror> Methods
		{
			get
			{
				if (methodsMirror == null)
					methodsMirror = Array.AsReadOnly (sdbType.GetMethods ().Select (m => Cache.Lookup<SdbMethodMirror> (m) as IMethodMirror).ToArray ());
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

		public override int GetHashCode ()
		{
			return (Assembly.FullName + "_" + "_" + FullName).GetHashCode ();
		}

		public override bool Equals (object o)
		{
			var right = o as SdbTypeMirror;
			if (right == null)
				return false;
			return this.GetHashCode () == right.GetHashCode ();
		}

	}
}
