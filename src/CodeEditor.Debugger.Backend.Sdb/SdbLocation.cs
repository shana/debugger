using CodeEditor.Debugger.Backend;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	public class SdbLocation : ILocation
	{
		public Location MDSLocation { get; private set; }

		public SdbLocation(Location location)
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
