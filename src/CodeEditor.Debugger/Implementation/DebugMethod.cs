using System.Linq;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	internal class DebugMethod : IDebugMethod
	{
		private readonly MethodMirror _methodMirror;
		private IDebugLocation[] _locations;

		public DebugMethod(MethodMirror methodMirror)
		{
			_methodMirror = methodMirror;
		}

		public IDebugLocation[] Locations
		{
			get
			{
				if (_locations != null)
					return _locations;
				_locations = _methodMirror.Locations.Select(DebugLocationFor).ToArray();
				return _locations;
			}
		}

		private IDebugLocation DebugLocationFor(Location l)
		{
			return new DebugLocation(l);
		}
	}
}