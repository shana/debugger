using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	internal class SdbMethodMirror : Wrapper, IMethodMirror
	{
		private MethodMirror _methodMirror { get { return _obj as MethodMirror; } }
		private readonly List<ILocation> _locations;

		public SdbMethodMirror(MethodMirror methodMirror) : base(methodMirror)
		{
			_locations = new List<ILocation>(_methodMirror.Locations.Select(SdbLocationFor));
		}

		public string FullName
		{
			get { return _methodMirror.Name; }
		}

		public IEnumerable<ILocation> Locations
		{
			get { return _locations.ToArray(); }
		}

		private static ILocation SdbLocationFor(Location l)
		{
			return new SdbLocation(l);
		}

	}
}
