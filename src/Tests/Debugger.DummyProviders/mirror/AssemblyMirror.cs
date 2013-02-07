using Debugger.Backend;
using Mono.Cecil;

namespace Debugger.DummyProviders
{
	class AssemblyMirror :BaseMirror, IAssemblyMirror
	{
		public string FullName { get; set; }
		public string Path { get; set; }
		public AssemblyDefinition Metadata { get; set; }


		public override int GetHashCode ()
		{
			return FullName.GetHashCode ();
		}

		public override bool Equals (object o)
		{
			var right = o as AssemblyMirror;
			if (right == null)
				return false;
			return this.GetHashCode () == right.GetHashCode ();
		}
	}
}
