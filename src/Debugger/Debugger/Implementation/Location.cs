using Debugger.Backend;

namespace Debugger.Implementation
{
	public class Location : Wrapper, ILocation
	{
		private static Location _default;
		public static Location Default { get { return _default; } }

		private readonly int _line;
		private readonly string _file;

		static Location()
		{
			_default = new Location(0, "");
		}

		public Location(int line,string file)
		{
			_line = line;
			_file = file;
		}

		public int LineNumber
		{
			get { return _line; }
		}

		public string File
		{
			get { return _file; }
		}

		public static ILocation FromLocation (Mono.Debugger.Soft.Location location)
		{
			return new Location(location.LineNumber, location.SourceFile);
		}
	}
}
