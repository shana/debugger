using System.Collections.Generic;
using Debugger.Backend;
using Mono.Cecil;

namespace Debugger.DummyProviders
{
	class MethodMirror : BaseMirror, IMethodMirror
	{
		private IList<ILocation> locations = new List<ILocation> ();
		public IList<ILocation> Locations { get { return locations; } }

		public string FullName { get; set; }
		public ITypeMirror DeclaringType { get; set; }
		public ITypeMirror ReturnType { get; set; }
		public string Name { get; set; }

		public int MetadataToken { get; set; }
		MethodDefinition metadata;
		MethodDefinition Metadata
		{
			get
			{
				if (metadata == null)
					metadata = DeclaringType.Assembly.Metadata.MainModule.LookupToken (MetadataToken) as MethodDefinition;
				return metadata;
			}
		}

		public MethodMirror (string fullName, string name, ITypeMirror type, ITypeMirror returnType, int token, MethodDefinition def, ILocation[] locations)
		{
			this.FullName = fullName;
			this.Name = name;
			this.DeclaringType = type;
			this.ReturnType = returnType;
			this.MetadataToken = token;
			this.metadata = def;
			this.locations = locations;
		}

		public override int GetHashCode ()
		{
			return (DeclaringType.Assembly.FullName + "_" + DeclaringType.FullName + "_" + FullName).GetHashCode ();
		}

		public override bool Equals (object o)
		{
			var right = o as MethodMirror;
			if (right == null)
				return false;
			return this.GetHashCode () == right.GetHashCode ();
		}

	}
}
