using System.Linq;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	public class BreakpointMediator
	{
		private readonly IVirtualMachine _vm;
		private readonly IBreakpointProvider _breakpointProvider;

		public BreakpointMediator (IVirtualMachine vm, IBreakpointProvider breakpointProvider)
		{
			_vm = vm;
			_breakpointProvider = breakpointProvider;

			_vm.OnTypeLoad += OnTypeLoad;
		}

		private void OnTypeLoad (TypeLoadEvent e)
		{
			var sourcefiles = e.Type.GetSourceFiles (true);

			if (e.Type.Name == "TestClass")
			{
				int a = 4;
			}

			var breakPoints = _breakpointProvider.Breakpoints;
			var relevantBreakPoints = breakPoints.Where (bp => sourcefiles.Contains (bp.File));

			var methodMirrors = e.Type.GetMethods ();
			foreach (var bp in relevantBreakPoints)
			{
				foreach (var method in methodMirrors) 
				{
					var bestLocation = BestLocationIn (method, bp);
					if (bestLocation == null)
						continue;

					_vm.CreateBreakpointRequest (bestLocation).Enable();
				}
			}
		}

		private Location BestLocationIn (MethodMirror method, IBreakPoint bp)
		{
			var locations = method.Locations.ToArray ();
			var name = method.FullName;

			return locations.FirstOrDefault (l => l.SourceFile == bp.File && l.LineNumber == bp.LineNumber);
		}
	}
}
