using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using CodeEditor.Composition;
using CodeEditor.Debugger.Unity.Engine;
using Mono.Debugger.Soft;
using UnityEngine;
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

		[ImportingConstructor]
		public MainWindow(SourceWindow sourceWindow, ConsoleWindow console)
		{
			_sourceWindow = sourceWindow;
			_console = console;
			AdjustLayout();

			QueueUserWorkItem(Connect);
		}

		public void OnGUI()
		{	
			if (GUILayout.Button(IsConnected ? "Detach" : "Attach"))
				ToggleConnectionState();
			_console.OnGUI();
			_sourceWindow.OnGUI();
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
			var running = true;
			while (running)
			{
				if (_vm == null)
					break;

				var e = _vm.GetNextEvent();
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
						continue;
					case EventType.VMDisconnect:
					case EventType.VMDeath:
						Trace(e.EventType.ToString());
						running = false;
						Dispose();
						continue;
				}
				SafeResume();
			}
		}

		private void OnMethodEntry(MethodEntryEvent e)
		{
			var locations = e.Method.Locations;
			if (locations.Count > 0)
				ShowSourceLocation(locations[0]);
			else
				SafeResume();
		}

		private void ShowSourceLocation(Location location)
		{
			Trace("{0}:{1}", location.SourceFile, location.LineNumber);
			_sourceWindow.ShowSourceLocation(location.SourceFile, location.LineNumber);
			_methodEntryRequest.Disable();
			SafeResume();
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
			WithErrorLogging(() => _vm.Resume());
		}

		private static bool IsUserCode(AssemblyMirror assembly)
		{
			return assembly.GetName().Name.StartsWith("Assembly-");
		}

		private void TraceError(Exception exception)
		{
			_console.WriteLine(exception.ToString());
		}

		private void Trace(string format, params object[] args)
		{
			_console.WriteLine(string.Format(format, args));
		}
	}
}
