using Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbLocation : Wrapper, ILocation
	{
		private readonly string sourceFile;
		private readonly int lineNumber;
		Location location { get { return obj as Location; } }

		public string SourceFile { get { return location != null ? location.SourceFile : sourceFile; } }
		public int LineNumber { get { return location != null ? location.LineNumber : lineNumber; } }

		public SdbLocation (string sourceFile, int lineNumber) : base (null)
		{
			this.sourceFile = sourceFile;
			this.lineNumber = lineNumber;
		}

		public SdbLocation (Location location) : base (location) { }
	}
}
