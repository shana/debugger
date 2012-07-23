using System;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger
{
    public interface IVirtualMachine
	{
		event Action OnVMGotSuspended;
        event Action<TypeLoadEvent> OnTypeLoad;
        BreakpointEventRequest CreateBreakpointRequest(Location location);
	}
}