using Debugger.Backend;

namespace Debugger
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

		public Location(Mono.Debugger.Soft.Location location) : base(location)
		{
			_line = location.LineNumber;
			_file = location.SourceFile;
		}

		public Location(int line,string file) : base(null)
		{
			_line = line;
			_file = file;
		}

		public int LineNumber
		{
			get { return _line; }
		}

		public string SourceFile
		{
			get { return _file; }
		}
	}
}
