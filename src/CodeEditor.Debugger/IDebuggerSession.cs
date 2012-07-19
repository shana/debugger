using System;
using System.Collections.Generic;
using CodeEditor.Debugger.Backend;
using CodeEditor.Debugger.Implementation;
using Mono.Debugger.Soft;
using AssemblyMirror = Mono.Debugger.Soft.AssemblyMirror;

namespace CodeEditor.Debugger
{
	public interface IDebuggerSession
	{
		bool Suspended { get; }
		IEnumerable<AssemblyMirror> LoadedAssemblies { get; }
		event Action<Event> VMGotSuspended;
		IList<ThreadMirror> GetThreads();
		void SafeResume();
		void SendStepRequest(StepDepth over);
		void Break();
		ThreadMirror GetMainThread();
		void Start(int debuggerPort);
		event Action<string> TraceCallback;
		void Disconnect();
		void Update();
		ThreadsRequest GetThreadsAsync();
		BreakpointEventRequest CreateBreakpointRequest(Location location);
		event Action<ITypeMirror> TypeLoaded;
		event Action<IAssemblyMirror> AssemblyLoaded;
		event Action<IAssemblyMirror> AssemblyUnloaded;
	}
}