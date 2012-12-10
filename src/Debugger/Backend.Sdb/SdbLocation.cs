using Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbLocation : Wrapper, ILocation
	{
		public Location MDSLocation { get { return _obj as Location; } }

		public string SourceFile { get { return MDSLocation.SourceFile; } }
		public int LineNumber { get { return MDSLocation.LineNumber; } }

		public SdbLocation(Location location) : base(location) {}
	}
}
