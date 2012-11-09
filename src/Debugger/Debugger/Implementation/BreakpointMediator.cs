using System.Linq;
using Debugger.Backend;
using MDS = Mono.Debugger.Soft;
using Mono.Debugger.Soft;

namespace Debugger.Implementation
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

		private void OnTypeLoad (ITypeLoadEvent e)
		{
			var sourcefiles = e.Type.SourceFiles;

			if (e.Type.Name == "TestClass")
			{
				int a = 4;
			}

			var breakPoints = _breakpointProvider.Breakpoints;
			var relevantBreakPoints = breakPoints.Where (bp => sourcefiles.Contains (bp.Location.File));

			foreach (var bp in relevantBreakPoints)
			{
				foreach (var method in e.Type.Methods)
				{
					var bestLocation = BestLocationIn (method, bp);
					if (bestLocation == null)
						continue;

					_vm.CreateBreakpointRequest (bestLocation).Enable();
				}
			}
		}

		private ILocation BestLocationIn (IMethodMirror method, IBreakPoint bp)
		{
			var locations = method.Locations;
			var name = method.FullName;

			return locations.FirstOrDefault (l => l.File == bp.Location.File && l.LineNumber == bp.Location.LineNumber);
		}
	}
}
