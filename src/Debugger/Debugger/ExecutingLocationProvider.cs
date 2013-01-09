using CodeEditor.Composition;
using Debugger.Backend;
using Mono.Debugger.Soft;

namespace Debugger
{
	public class ExecutingLocationProvider : IExecutingLocationProvider
	{
		private Location _currentLocation;
		private readonly IVirtualMachine _vm;

		public ExecutingLocationProvider (IVirtualMachine vm)
		{
			_vm = vm;
			_vm.OnVMGotSuspended += OnVMGotSuspended;
			_currentLocation = Debugger.Location.Default;
		}

		private void OnVMGotSuspended(IEvent suspendingEvent)
		{
			if (suspendingEvent is VMDeathEvent)
				return;

			var frames = suspendingEvent.Thread.Unwrap<ThreadMirror>().GetFrames();

			_currentLocation = frames.Length == 0
						? new Location(0, "")
						: new Location(frames[0].Location);
		}

		public ILocation Location
		{
			get { return _currentLocation; }
		}
	}
}
