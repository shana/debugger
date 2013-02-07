using System;
using Debugger.Backend;

namespace Debugger.DummyProviders
{
	class Location : BaseMirror, ILocation
	{
		public string SourceFile { get; set; }
		public int LineNumber { get; set; }

		public Location (string file, int line)
		{
			SourceFile = file;
			LineNumber = line;
		}

		public override bool Equals (object obj)
		{
			var loc = obj as Location;
			if (loc == null)
				return false;
			return loc.SourceFile == SourceFile && loc.LineNumber == LineNumber;
		}
	}
}
