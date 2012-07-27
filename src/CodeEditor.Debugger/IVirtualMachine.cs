using System;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger
{
	public interface IVirtualMachine
	{
		event Action<Event> OnVMGotSuspended;
		event Action<TypeLoadEvent> OnTypeLoad;
		BreakpointEventRequest CreateBreakpointRequest (Location location);
	}
}