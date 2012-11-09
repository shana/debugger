using Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbLocation : Wrapper, ILocation
	{
		public Location MDSLocation { get; private set; }

		public SdbLocation(Location location) : base(location)
		{
			MDSLocation = location;
		}

		public string File { get { return MDSLocation.SourceFile; } }

		public int LineNumber { get { return MDSLocation.LineNumber; } }

	}
}
