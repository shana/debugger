using System;
using System.Collections.Generic;

namespace Debugger.Backend
{
	public interface IVirtualMachine : IWrapper
	{
		event Action<IEvent> OnVM;
		event Action<IEvent> OnVMSuspended;
		event Action<IEvent> OnAppDomain;
		event Action<IEvent> OnThread;
		event Action<IAssemblyEvent> OnAssembly;
		event Action<ITypeEvent> OnType;
		event Action<IBreakpointEvent> OnBreakpoint;

		IEnumerable<IAssemblyMirror> Assemblies { get; }
		IEnumerable<IAssemblyMirror> RootAssemblies { get; }

		void Attach (int port);

		//IBreakpoint SetBreakpoint (ILocation location);
		void ClearAllBreakpoints ();
		IList<IThreadMirror> GetThreads();

		void Suspend ();
		void Resume ();

		void Detach ();
		event Action<IEvent> OnStep;
	}
}
