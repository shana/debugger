namespace CodeEditor.Debugger.Implementation
{
	class Location : ILocation
	{
		private readonly int _line;
		private readonly string _file;

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