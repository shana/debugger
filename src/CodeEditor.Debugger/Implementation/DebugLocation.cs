using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeEditor.Debugger.Implementation
{
	class DebugLocation : IDebugLocation
	{
		private readonly string _file;
		private int _line;

		public DebugLocation(string file, int line)
		{
			_file = file;
			_line = line;
		}

		public string File
		{
			get { return _file; }
		}

		public int LineNumber
		{
			get { return _line; }
		}
	}
}
