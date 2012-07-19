using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	class DebugLocation : IDebugLocation
	{
		public Location MDSLocation { get; private set; }

		public DebugLocation(Location location)
		{
			MDSLocation = location;
		}

		public string File
		{
			get { return MDSLocation.SourceFile; }
		}

		public int LineNumber
		{
			get { return MDSLocation.LineNumber; }
		}

	}
}
