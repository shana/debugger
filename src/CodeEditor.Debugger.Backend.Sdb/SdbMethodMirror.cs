using System.Linq;
using CodeEditor.Debugger.Backend;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	internal class SdbMethodMirror : IMethodMirror
	{
		private readonly MethodMirror _methodMirror;
		private ILocation[] _locations;

		public SdbMethodMirror(MethodMirror methodMirror)
		{
			_methodMirror = methodMirror;
		}

		public ILocation[] Locations
		{
			get
			{
				if (_locations != null)
					return _locations;
				_locations = _methodMirror.Locations.Select(DebugLocationFor).ToArray();
				return _locations;
			}
		}

		private ILocation DebugLocationFor(Location l)
		{
			return new SdbLocation(l);
		}
	}
}