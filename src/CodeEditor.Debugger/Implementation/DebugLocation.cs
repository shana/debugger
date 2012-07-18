using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeEditor.Debugger.Implementation
{
	class DebugLocation : IDebugLocation
	{
		private readonly string _file;

		public DebugLocation(string file)
		{
			_file = file;
		}

		public string File
		{
			get { return _file; }
		}
	}
}
