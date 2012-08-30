using System.Linq;
using CodeEditor.Composition;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Backend.Sdb
{
	internal class SdbMethodMirror : IMethodMirror
	{
		private readonly MethodMirror _methodMirror;
		private readonly Lazy<ILocation[]> _locations;

		public SdbMethodMirror(MethodMirror methodMirror)
		{
			_methodMirror = methodMirror;
			_locations = new Lazy<ILocation[]>(() => _methodMirror.Locations.Select(SdbLocationFor).ToArray());
		}

		public ILocation[] Locations
		{
			get { return _locations.Value; }
		}

		private static ILocation SdbLocationFor(Location l)
		{
			return new SdbLocation(l);
		}
	}
}