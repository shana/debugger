using Debugger.Backend;

namespace Debugger
{
	public class Location : Wrapper, ILocation
	{
		private static Location defaultLocation;
		public static Location Default { get { return defaultLocation; } }

		private readonly int line;
		private readonly string file;

		static Location ()
		{
			defaultLocation = new Location ("", 0);
		}

		public Location (string file, int line)
			: base (null)
		{
			this.line = line;
			this.file = file;
		}

		public int LineNumber
		{
			get { return line; }
		}

		public string SourceFile
		{
			get { return file; }
		}
	}
}
