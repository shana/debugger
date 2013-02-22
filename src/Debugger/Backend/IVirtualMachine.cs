using System;
using System.Collections.Generic;

namespace Debugger.Backend
{
	public interface IVirtualMachine : IWrapper
	{
		IList<IAssemblyMirror> Assemblies { get; }
		IList<IAssemblyMirror> RootAssemblies { get; }
		IList<IThreadMirror> Threads { get; } 

		event Action<IEvent> VMStateChanged;
		event Action<IEvent> VMSuspended;
		event Action<IEvent> AppDomainLoaded;
		event Action<IEvent> AppDomainUnloaded;
		event Action<IEvent> ThreadStarted;
		event Action<IEvent> ThreadStopped;
		event Action<IAssemblyEvent> AssemblyLoaded;
		event Action<IAssemblyEvent> AssemblyUnloaded;
		event Action<ITypeEvent> TypeLoaded;
		event Action<IBreakpointEvent> BreakpointHit;
		event Action<IEvent> Stepped;

		void Attach (int port);
		void Detach ();

		void Suspend ();
		void Resume ();

		void Suspend (IEvent ev);
	}
}
