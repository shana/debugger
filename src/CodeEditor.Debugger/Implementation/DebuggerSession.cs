using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using CodeEditor.Composition;
using CodeEditor.Debugger.Backend;
using CodeEditor.Debugger.Backend.Sdb;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	[Export(typeof(IDebuggerSession))]
	internal class DebuggerSession : IDebuggerSession
	{
		private int _debuggerPort;
		private volatile VirtualMachine _vm;
		private MethodEntryEventRequest _methodEntryRequest;
		private bool _vmSuspended;
		private readonly Queue<Event> _queuedEvents = new Queue<Event>();
		private EventRequest _requestWaitingForResponse;

		public event Action<Event> VMGotSuspended;

		public event Action<string> TraceCallback ;
		private ThreadMirror _mainThread;
		private readonly List<AssemblyMirror> _loadedAssemblies = new List<AssemblyMirror>();
		private Dictionary<Location,BreakpointEventRequest> _breakpointEventRequests = new Dictionary<Location, BreakpointEventRequest>();
		private readonly List<SdbAssemblyMirror> _debugAssemblies = new List<SdbAssemblyMirror>();

		public void Start(int debuggerPort)
		{
			_debuggerPort = debuggerPort;
			QueueUserWorkItem(Connect);
		}

		public IEnumerable<Mono.Debugger.Soft.AssemblyMirror> LoadedAssemblies { get { return _loadedAssemblies; } } 

		public void Connect()
		{
			WithErrorLogging(() =>
			{
				Trace("Attempting connection at port {0}...", _debuggerPort);

				_vm = VirtualMachineManager.Connect(new IPEndPoint(IPAddress.Loopback, _debuggerPort));
				_vm.Suspend();
				_vm.EnableEvents(
					EventType.AssemblyLoad,
					EventType.AssemblyUnload,
					EventType.AppDomainUnload,
					EventType.AppDomainCreate,
					EventType.VMDeath,
					EventType.VMDisconnect,
					EventType.TypeLoad
					);
				_methodEntryRequest = _vm.CreateMethodEntryRequest();
				StartEventLoop();
			});
		}

		public bool IsConnected
		{
			get { return _vm != null; }
		}

		public bool WaitingForResponse { get { return _requestWaitingForResponse != null; } }
		public bool Suspended { get { return _vmSuspended; } }

		private void ProcessQueuedEvents()
		{
			lock (_queuedEvents)
			{
				while (_queuedEvents.Count > 0)
				{
					var e = _queuedEvents.Dequeue();
					HandleEvent(e);
				}
			}
		}

		public void Disconnect()
		{
			if (_vm == null) return;
			WithErrorLogging(() => _vm.Disconnect());
			Dispose();
		}

		private void Dispose()
		{
			if (_vm == null) return;
			WithErrorLogging(() => _vm.Dispose());
			_vm = null;
		}

		private void StartEventLoop()
		{
			QueueUserWorkItem(EventLoop);
		}

		private void QueueUserWorkItem(Action eventLoop)
		{
			ThreadPool.QueueUserWorkItem(_ => WithErrorLogging(eventLoop));
		}

		private void WithErrorLogging(Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				TraceError(e);
			}
		}

		private void EventLoop()
		{
			while (true)
			{
				if (_vm == null)
					break;

				var e = _vm.GetNextEvent();
				lock (_queuedEvents)
				{
					_queuedEvents.Enqueue(e);
				}
			}
		}

		private void HandleEvent(Event e)
		{
			if (_mainThread == null)
				_mainThread = e.Thread;
			switch (e.EventType)
			{
				case EventType.VMStart:
					OnVMStart((VMStartEvent) e);
					return;
				case EventType.AssemblyLoad:
					OnAssemblyLoad((AssemblyLoadEvent)e);
					break;
				case EventType.AssemblyUnload:
					OnAssemblyUnload((AssemblyUnloadEvent)e);
					break;
				case EventType.AppDomainCreate:
					OnAppDomainCreate((AppDomainCreateEvent)e);
					break;
				case EventType.AppDomainUnload:
					OnAppDomainUnload((AppDomainUnloadEvent)e);
					break;
				case EventType.TypeLoad:
					OnTypeLoad((TypeLoadEvent)e);
					break;
				case EventType.MethodEntry:
					OnMethodEntry((MethodEntryEvent)e);
					return;
				case EventType.Step:
					OnStep((StepEvent)e);
					return;
				case EventType.VMDisconnect:
				case EventType.VMDeath:
					Trace(e.EventType.ToString());
					_vmSuspended = true;
					Dispose();
					return;
			}
			SafeResume();
		}

		private void OnVMStart(VMStartEvent vmStartEvent)
		{
			SafeResume();
		}

		private void OnStep(StepEvent stepEvent)
		{
			Trace("OnStep event");
			stepEvent.Request.Disable();
			_requestWaitingForResponse = null;
			OnVMGotSuspended(stepEvent);
		}

		private void OnMethodEntry(MethodEntryEvent e)
		{
			Trace("OnMethodEntry event");
			_methodEntryRequest.Disable();
			OnVMGotSuspended(e);
		}

		private void OnVMGotSuspended(Event e)
		{
			_vmSuspended = true;

			if (VMGotSuspended != null)
				VMGotSuspended(e);
		}


		private void OnAppDomainUnload(AppDomainUnloadEvent appDomainUnloadEvent)
		{
			Trace("AppDomainUnload: {0}", appDomainUnloadEvent.Domain.FriendlyName);
		}

		private void OnAppDomainCreate(AppDomainCreateEvent appDomainCreateEvent)
		{
			Trace("AppDomainCreate: {0}", appDomainCreateEvent.Domain.FriendlyName);
		}

		private void OnAssemblyLoad(AssemblyLoadEvent e)
		{
			var assembly = e.Assembly;
			ProcessLoadedAssembly(assembly);
			var debugAssembly = DebugAssemblyFor(assembly);

			AssemblyUnloaded(debugAssembly);
		}

		private void ProcessLoadedAssembly(Mono.Debugger.Soft.AssemblyMirror assembly)
		{
			var hasDebugSymbols = HasDebugSymbols(assembly);
			Trace("AssemblyLoad: {0}", assembly.GetName().FullName);
			Trace("\tHasDebugSymbols: {0}", hasDebugSymbols);
			
			if (!hasDebugSymbols || !IsUserCode(assembly)) return;

			_loadedAssemblies.Add(assembly);

			var wasEnabled = _methodEntryRequest.Enabled;
			_methodEntryRequest.Disable();
			_methodEntryRequest.AssemblyFilter = _loadedAssemblies;
			if (wasEnabled)
				_methodEntryRequest.Enable();
		}

		private static bool IsUserCode(Mono.Debugger.Soft.AssemblyMirror assembly)
		{
			return assembly.GetName().Name.StartsWith("SdbAssemblyMirror-");
		}

		private static bool HasDebugSymbols(Mono.Debugger.Soft.AssemblyMirror assembly)
		{
			return File.Exists(assembly.ManifestModule.FullyQualifiedName + ".mdb");
		}

		private void OnAssemblyUnload(AssemblyUnloadEvent e)
		{
			_loadedAssemblies.Remove(e.Assembly);

			var wasEnabled = _methodEntryRequest.Enabled;
			_methodEntryRequest.Disable();
			_methodEntryRequest.AssemblyFilter = _loadedAssemblies;
			if (wasEnabled)
				_methodEntryRequest.Enable();

			_breakpointEventRequests = _breakpointEventRequests.Where(kvp => kvp.Key.Method.DeclaringType.Assembly != e.Assembly).ToDictionary(kvp=>kvp.Key,kvp=>kvp.Value);

			Trace("AssemblyUnload: {0}", e.Assembly.GetName().FullName);

			AssemblyUnloaded(DebugAssemblyFor(e.Assembly));
		}

		private IAssemblyMirror DebugAssemblyFor(Mono.Debugger.Soft.AssemblyMirror assemblyMirror)
		{
			var debugAssembly = _debugAssemblies.SingleOrDefault(da => da.Mirror == assemblyMirror);
			if (debugAssembly != null)
				return debugAssembly;

			debugAssembly = new SdbAssemblyMirror(assemblyMirror);
			_debugAssemblies.Add(debugAssembly);
			return debugAssembly;
		}

		private void OnTypeLoad(TypeLoadEvent e)
		{
			var debugAssembly = DebugAssemblyFor(e.Type.Assembly);
			var debugType = new SdbTypeMirror(e.Type,debugAssembly);

			TypeLoaded(debugType);
		}

		public void SafeResume()
		{
			Trace("SafeResume");
			_vmSuspended = false;
			WithErrorLogging(() => _vm.Resume());
		}

		public void Break()
		{
			_methodEntryRequest.Enable();
		}

		public void SendStepRequest(StepDepth stepDepth)
		{
			var stepEventRequest = _vm.CreateStepRequest(_mainThread);
			stepEventRequest.Depth = stepDepth;
			stepEventRequest.Size = StepSize.Line;
			stepEventRequest.Enable();
			SafeResume();
			_requestWaitingForResponse = stepEventRequest;
		}

		public IList<ThreadMirror> GetThreads()
		{
			return _vm.GetThreads();
		}

		private void TraceError(Exception exception)
		{
			Trace("error: " + exception);
			Trace("stacktrace" + exception.StackTrace);
		}

		public void Update()
		{
			ProcessQueuedEvents();
		}

		public ThreadsRequest GetThreadsAsync()
		{
			return new ThreadsRequest();
		}

		public BreakpointEventRequest CreateBreakpointRequest(Location location)
		{
			var request = _vm.CreateBreakpointRequest(location);
			_breakpointEventRequests[location] = request;
			return request;
		}

		public event Action<ITypeMirror> TypeLoaded;
		public event Action<IAssemblyMirror> AssemblyLoaded;
		public event Action<IAssemblyMirror> AssemblyUnloaded;

		private void Trace(string format, params object[] args)
		{
			var text = String.Format(format, args);
			TraceCallback(text);
		}

		public ThreadMirror GetMainThread()
		{
			return _mainThread;
		}
	}
}
