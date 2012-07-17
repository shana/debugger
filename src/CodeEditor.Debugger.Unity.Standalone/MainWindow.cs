using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Debugger.Unity.Engine;
using UnityEngine;
using Event = Mono.Debugger.Soft.Event;
using EventType = UnityEngine.EventType;

namespace CodeEditor.Debugger.Unity.Standalone
{
	[Export]
	class MainWindow
	{
		private readonly SourceWindow _sourceWindow;
		private readonly LogWindow _log;

		private readonly IDebuggerSession _debuggingSession;
		private readonly DebuggerWindowManager _windowManager;
		private readonly ISourceNavigator _sourceNavigator;
		private readonly int _debugeeProcessID;

		[ImportingConstructor]
		public MainWindow(SourceWindow sourceWindow, LogWindow log, DebuggerWindowManager windowManager, ISourceNavigator sourceNavigator, IDebuggerSession debuggingSession)
		{
			_sourceWindow = sourceWindow;
			_log = log;
			_windowManager = windowManager;
			_sourceNavigator = sourceNavigator;
			_debuggingSession = debuggingSession;

			Camera.main.backgroundColor = new Color(0.125f,0.125f,0.125f,0);
			Application.runInBackground = true;
			
			_debuggingSession.TraceCallback += s => Trace(s);
			_debuggingSession.Start(DebuggerPortFromCommandLine());
			_debuggingSession.VMGotSuspended += OnVMGotSuspended;

			_debugeeProcessID = DebugeeProcessIDFromCommandLine();

			SetupDebuggingWindows();

			AdjustLayout();
		}



		private void SetupDebuggingWindows()
		{
			/*
			_windowManager.Add(new ExecutionFlowControlWindow(_debuggingSession));
			_windowManager.Add(new CallStackDisplay(_debuggingSession, _sourceNavigator));
			_windowManager.Add(new ThreadsDisplay(_debuggingSession, new DebugThreadProvider(_debuggingSession)));

			_windowManager.Add(_log);*/
		}


		private void OnVMGotSuspended(Event e)
		{
			var stackFrames = _debuggingSession.GetMainThread().GetFrames();
			if (!stackFrames.Any()) return;
			
			var topFrame = stackFrames[0];
			_sourceNavigator.ShowSourceLocation(topFrame.Location);
		}

		public void OnGUI()
		{
			if (!DebugeeProcessAlive())
				Application.Quit();

			if (UnityEngine.Event.current.type == EventType.Layout)
				_debuggingSession.Update();

			_windowManager.OnGUI();
			_sourceWindow.OnGUI();
		}

		private bool DebugeeProcessAlive()
		{
			Process process;
			try
			{
				process = Process.GetProcessById(_debugeeProcessID);
			}
			catch (ArgumentException)
			{
				return false;
			}
			return !process.HasExited;
		}

		private void AdjustLayout()
		{
			var srcViewPort = new Rect(0, 0, Screen.width, Screen.height * .7f);
			_sourceWindow.ViewPort = srcViewPort;

			var consoleTop = srcViewPort.yMax + VerticalSpacing;
			_windowManager.ViewPort = new Rect(0, consoleTop, Screen.width, Screen.height - consoleTop);
		}

		const int VerticalSpacing = 4;

		private int DebuggerPortFromCommandLine()
		{
			return ReadIntFromCommandLine(1);
		}

		private int DebugeeProcessIDFromCommandLine()
		{
			return ReadIntFromCommandLine(2);
		}

		private static int ReadIntFromCommandLine(int index)
		{
			var args = Environment.GetCommandLineArgs();
			return int.Parse(args[index]);
		}

		private void Trace(string format, params object[] args)
		{
			var text = string.Format(format, args);
			Console.WriteLine(text);
			_log.WriteLine(text);
		}

		public void OnApplicationQuit()
		{
			_debuggingSession.Disconnect();
		}
	}
}
