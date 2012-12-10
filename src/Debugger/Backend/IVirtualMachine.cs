using System;
using System.Collections.Generic;

namespace Debugger.Backend
{
	public interface IVirtualMachine : IWrapper
	{
		event Action<IEvent> OnVMGotSuspended;
		event Action<ITypeLoadEvent> OnTypeLoad;
		IBreakpointEventRequest CreateBreakpointRequest (ILocation location);
		IList<IThreadMirror> GetThreads();
	}
}
