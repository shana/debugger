using CodeEditor.Composition;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	using L = Location;

	[Export(typeof(IExecutingLocationProvider))]
	public class ExecutingLocationProvider : IExecutingLocationProvider
	{
		private Location _currentLocation;
		private readonly IVirtualMachine _vm;

		[ImportingConstructor]
		public ExecutingLocationProvider(IVirtualMachine vm)
		{
			_vm = vm;
			_vm.OnVMGotSuspended += OnVMGotSuspended;
			_currentLocation = L.Default;
		}

		private void OnVMGotSuspended(Event suspendingEvent)
		{
			if (suspendingEvent is VMDeathEvent)
				return;

			var frames = suspendingEvent.Thread.GetFrames();



			_currentLocation = frames.Length == 0
						? new Location(0, "")
						: new Location(frames[0].LineNumber, frames[0].Location.SourceFile);
		}

		public ILocation Location
		{
			get { return _currentLocation; }
		}
	}
}