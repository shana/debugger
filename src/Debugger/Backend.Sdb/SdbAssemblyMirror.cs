using Mono.Cecil;
using Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbAssemblyMirror : Wrapper, IAssemblyMirror
	{
		private AssemblyDefinition metadata;

		AssemblyMirror mirror { get { return obj as AssemblyMirror; } }
		public string FullName { get { return mirror.GetName ().FullName; } }
		public string Path { get { return mirror.Location; }}

		public AssemblyDefinition Metadata
		{
			get
			{
				if (metadata == null) {
					metadata = AssemblyDefinition.ReadAssembly (mirror.Location);
				}
				return metadata;
			}
			set { metadata = value; }
		}

		public SdbAssemblyMirror (AssemblyMirror assemblyMirror) : base (assemblyMirror) { }

		public override string ToString ()
		{
			return string.Format("({0}) {1}", mirror.Id, FullName);
		}

		public override int GetHashCode ()
		{
			return FullName.GetHashCode ();
		}

		public override bool Equals (object o)
		{
			var right = o as SdbAssemblyMirror;
			if (right == null)
				return false;
			return this.GetHashCode () == right.GetHashCode ();
		}
	}
}
