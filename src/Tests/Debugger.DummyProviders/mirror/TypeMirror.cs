using System;
using System.Collections.Generic;
using Debugger.Backend;
using Mono.Cecil;
using System.Linq;

namespace Debugger.DummyProviders
{
	internal class TypeMirror : BaseMirror, ITypeMirror
	{
		private IList<string> sourceFiles;
		private IList<IMethodMirror> methods;

		public IList<string> SourceFiles
		{
			get
			{
				if (sourceFiles == null)
					sourceFiles = LoadSourceFiles ().AsReadOnly ();
				return sourceFiles;
			}
		}

		public IList<IMethodMirror> Methods
		{
			get
			{
				if (methods == null)
					methods = LoadMethods ().AsReadOnly ();
				return methods;
			}
		}

		public IAssemblyMirror Assembly { get; set; }
		public string FullName { get; set; }
		public string Name { get; set; }

		public int MetadataToken { get; set; }
		TypeDefinition metadata;
		public TypeDefinition Metadata
		{
			get
			{
				if (metadata == null)
					metadata = Assembly.Metadata.MainModule.LookupToken (MetadataToken) as TypeDefinition;
				return metadata;
			}
			set { metadata = value; }
		}

		List<string> LoadSourceFiles ()
		{
			return Methods.SelectMany (m => m.Locations.Select (l => l.SourceFile)).Distinct().ToList ();
		}

		List<IMethodMirror> LoadMethods ()
		{
			return Metadata.Methods.Select (m =>
				new MethodMirror (m.FullName, m.Name, this, m.MetadataToken.ToInt32 (), m,
					new List<ILocation> (m.Body.Instructions.Where (i => i.SequencePoint != null).Select (il =>
						new Location (il.SequencePoint.Document.Url, il.SequencePoint.StartLine) as ILocation))
					.ToArray ()
			) as IMethodMirror).ToList ();
		}

		public override int GetHashCode ()
		{
			return (Assembly.FullName + "_" + "_" + FullName).GetHashCode ();
		}

		public override bool Equals (object o)
		{
			var right = o as TypeMirror;
			if (right == null)
				return false;
			return this.GetHashCode () == right.GetHashCode ();
		}
	
	}
}
