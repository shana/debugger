using System;
using System.Collections.Generic;
using CodeEditor.Composition;

namespace Debugger.Backend
{
	public interface IVirtualMachine : IWrapper
	{
		event Action<IEvent> OnVMStart;
		event Action<IEvent> OnVMGotSuspended;
		event Action<IEvent> OnBreakpoint;
		event Action<ITypeLoadEvent> OnTypeLoad;
		event Action<IAssemblyLoadEvent> OnAssemblyLoad;

		IBreakpointEventRequest CreateBreakpointRequest (ILocation location);
		IList<IThreadMirror> GetThreads();

		void Suspend ();
		void Resume ();
	}
}
