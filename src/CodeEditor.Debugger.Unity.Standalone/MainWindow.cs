using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using CodeEditor.Composition;
using CodeEditor.Debugger.Unity.Engine;
using Mono.Debugger.Soft;
using UnityEngine;
using Event = Mono.Debugger.Soft.Event;
using EventType = Mono.Debugger.Soft.EventType;

namespace CodeEditor.Debugger.Unity.Standalone
{
	[Export]
	class MainWindow
	{
		private readonly SourceWindow _sourceWindow;
		private readonly ConsoleWindow _console;
		private volatile VirtualMachine _vm;
		private MethodEntryEventRequest _methodEntryRequest;
		private bool _vmSuspended;
		private readonly CallStackDisplay _callStackDisplay;
		private EventRequest _requestWaitingForResponse;
		private Event _vmSuspendingEvent;
		private readonly Queue<Event> _queuedEvents = new Queue<Event>();

		[ImportingConstructor]
		public MainWindow(SourceWindow sourceWindow, ConsoleWindow console)
		{
			_sourceWindow = sourceWindow;
			_console = console;
			_callStackDisplay = new CallStackDisplay(frame => ShowSourceLocation(frame.Location));

			AdjustLayout();

			QueueUserWorkItem(Connect);
		}

		public void OnGUI()
		{
			ProcessQueuedEvents();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button(IsConnected ? "Detach" : "Attach"))
				ToggleConnectionState();

			DoExecutionFlowUI();
			GUILayout.EndHorizontal();

			_callStackDisplay.OnGUI();
			_console.OnGUI();
			_sourceWindow.OnGUI();
		}

		private void ProcessQueuedEvents()
		{
			lock (_queuedEvents)
			{
				while(_queuedEvents.Count>0)
				{
					var e = _queuedEvents.Dequeue();
					HandleEvent(e);
				}
			}
		}

		private void DoExecutionFlowUI()
		{
			GUI.enabled = _vmSuspended && _requestWaitingForResponse == null;
			if (GUILayout.Button("Continue"))
				SafeResume();
			if (GUILayout.Button("Step Over"))
				SendStepRequest(StepDepth.Over);
			if (GUILayout.Button("Step In"))
				SendStepRequest(StepDepth.Into);

			GUI.enabled = !_vmSuspended && _requestWaitingForResponse == null;
			if (GUILayout.Button("Break"))
				SendBreakRequeest();
			
			GUI.enabled = true;
		}

		private void SendBreakRequeest()
		{
			//cant figure out how to do this yet...
			throw new NotImplementedException();
		}

		private void SendStepRequest(StepDepth stepDepth)
		{
			var stepEventRequest = _vm.CreateStepRequest(_vmSuspendingEvent.Thread);
			stepEventRequest.Depth = stepDepth;
			stepEventRequest.Size = StepSize.Line;
			stepEventRequest.Enable();
			SafeResume();
			_requestWaitingForResponse = stepEventRequest;
		}

		private void AdjustLayout()
		{
			var srcViewPort = new Rect(0, ToolbarHeight, Screen.width, Screen.height * .6f - ToolbarHeight);
			_sourceWindow.ViewPort = srcViewPort;

			var consoleTop = srcViewPort.yMax + VerticalSpacing;
			_console.ViewPort = new Rect(0, consoleTop, Screen.width, Screen.height - consoleTop);
		}

		const int ToolbarHeight = 20;
		const int VerticalSpacing = 4;

		private bool IsConnected
		{
			get { return _vm != null; }
		}

		private void ToggleConnectionState()
		{
			if (IsConnected)
				Disconnect();
			else
				Connect();
		}

		void Connect()
		{
			Connect(DebuggerPortFromCommandLine());
		}

		private int DebuggerPortFromCommandLine()
		{
			var args = Environment.GetCommandLineArgs();
			return int.Parse(args[args.Length - 1]);
		}

		void Connect(int debuggerPort)
		{
			WithErrorLogging(() =>
			{
				Trace("Attempting connection at port {0}...", debuggerPort);

				_vm = VirtualMachineManager.Connect(new IPEndPoint(IPAddress.Loopback, debuggerPort));
				_vm.Suspend();
				_vm.EnableEvents(
					EventType.AssemblyLoad,
					EventType.AssemblyUnload,
					EventType.AppDomainUnload,
					EventType.AppDomainCreate,
					EventType.VMDeath,
					EventType.VMDisconnect);
				_methodEntryRequest = _vm.CreateMethodEntryRequest();
				StartEventLoop();
			});
		}

		private void Disconnect()
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
				lock(_queuedEvents)
				{
					_queuedEvents.Enqueue(e);
				}
			}
		}

		private void HandleEvent(Event e)
		{
			Trace(e.ToString());
			switch (e.EventType)
			{
				case EventType.VMStart:
					break;
				case EventType.AssemblyLoad:
					OnAssemblyLoad((AssemblyLoadEvent) e);
					break;
				case EventType.AssemblyUnload:
					OnAssemblyUnload((AssemblyUnloadEvent) e);
					break;
				case EventType.AppDomainCreate:
					OnAppDomainCreate((AppDomainCreateEvent) e);
					break;
				case EventType.AppDomainUnload:
					OnAppDomainUnload((AppDomainUnloadEvent) e);
					break;
				case EventType.TypeLoad:
					OnTypeLoad((TypeLoadEvent) e);
					break;
				case EventType.MethodEntry:
					OnMethodEntry((MethodEntryEvent) e);
					return;
				case EventType.Step:
					OnStep((StepEvent) e);
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

		private void OnStep(StepEvent stepEvent)
		{
			Trace("OnStep event");
			_requestWaitingForResponse = null;
			VmGotSuspended(stepEvent);
		}

		private void OnMethodEntry(MethodEntryEvent e)
		{
			Trace("OnMethodEntry event");
			_methodEntryRequest.Disable();
			VmGotSuspended(e);
		}

		private void VmGotSuspended(Event e)
		{
			_vmSuspendingEvent = e;
			_vmSuspended = true;
			var stackFrames = e.Thread.GetFrames();
			_callStackDisplay.SetCallFrames(stackFrames);

			var topFrame = stackFrames[0];
			ShowSourceLocation(topFrame.Location);
		}

		private void ShowSourceLocation(Location location)
		{
			if (!IsValidLocation(location))
				return;
			Trace("{0}:{1}", location.SourceFile, location.LineNumber);
			_sourceWindow.ShowSourceLocation(location.SourceFile, location.LineNumber);
		}

		private static bool IsValidLocation(Location location)
		{
			return location.LineNumber >= 1 && File.Exists(location.SourceFile);
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
			var hasDebugSymbols = HasDebugSymbols(assembly);
			Trace("AssemblyLoad: {0}", assembly.GetName().FullName);
			Trace("\tHasDebugSymbols: {0}", hasDebugSymbols);
			
			if (hasDebugSymbols && IsUserCode(assembly))
			{
				_methodEntryRequest.Disable();
				if (_methodEntryRequest.AssemblyFilter != null)
					_methodEntryRequest.AssemblyFilter.Add(assembly);
				else
					_methodEntryRequest.AssemblyFilter = new List<AssemblyMirror> { assembly };
				_methodEntryRequest.Enable();
			}
		}

		private void OnAssemblyUnload(AssemblyUnloadEvent e)
		{
			Trace("AssemblyUnload: {0}", e.Assembly.GetName().FullName);
		}

		private static bool HasDebugSymbols(AssemblyMirror assembly)
		{
			return File.Exists(assembly.ManifestModule.FullyQualifiedName + ".mdb");
		}

		private void OnTypeLoad(TypeLoadEvent e)
		{
			Trace("TypeLoad: {0}", e.Type.FullName);
		}

		private void SafeResume()
		{
			Trace("SafeResume");
			_vmSuspended = false;
			WithErrorLogging(() => _vm.Resume());
		}

		private static bool IsUserCode(AssemblyMirror assembly)
		{
			return assembly.GetName().Name.StartsWith("Assembly-");
		}

		private void TraceError(Exception exception)
		{
			_console.WriteLine("error: "+exception);
			_console.WriteLine("stacktrace"+exception.StackTrace);
		}

		private void Trace(string format, params object[] args)
		{
			var text = string.Format(format, args);
			Console.WriteLine(text);
			_console.WriteLine(text);
		}
	}
}
