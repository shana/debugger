namespace CodeEditor.Debugger.Implementation
{
	class Location : ILocation
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

		public string SourceFile
		{
			get { return _file; }
		}
	}
}