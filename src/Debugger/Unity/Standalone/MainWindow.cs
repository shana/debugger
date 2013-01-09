using System;
using System.Diagnostics;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Remoting;
using Debugger.Backend;
using Debugger.Unity.Engine;
using UnityEngine;
using Event = Mono.Debugger.Soft.Event;

namespace Debugger.Unity.Standalone
{
	[Export(typeof(IDebuggerSession))]
	class DummyDebuggerSession : IDebuggerSession
	{
		public bool Active { get; private set; }
		public IVirtualMachine VM { get; private set; }
		public event Action<string> TraceCallback;
	}

	[Export]
	public class MainWindow
	{
		private readonly SourceWindow _sourceWindow;
		private readonly LogWindow _log;
		private readonly SourcesWindow _sourcesWindow;

		private readonly IDebuggerSession _debuggingSession;
		private readonly DebuggerWindowManager _windowManager;
		private readonly ISourceNavigator _sourceNavigator;
		
		private readonly int _debugeeProcessID;

		[ImportingConstructor]
		public MainWindow (SourceWindow sourceWindow,
			LogWindow log,
			DebuggerWindowManager windowManager,
			ISourceNavigator sourceNavigator,
			IDebuggerSession debuggingSession,
			ISourcesProvider provider)
		{

			UnityEngine.Debug.Log ("MainWindow");

			_sourceWindow = sourceWindow;
			_log = log;
			_windowManager = windowManager;
			_sourceNavigator = sourceNavigator;

			if (HasArguments ())
				_debuggingSession = DebuggerSession.Attach (SdbPortFromCommandLine());
			else
				_debuggingSession = debuggingSession;
			_sourcesWindow = windowManager.Get<SourcesWindow>();

			Camera.main.backgroundColor = new Color(0.125f, 0.125f, 0.125f, 0);
			Application.runInBackground = true;

			SetupDebuggingWindows();

			AdjustLayout();

			if (!HasArguments())
				return;

			_debuggingSession.TraceCallback += s => Trace(s);
//			_debuggingSession.VMGotSuspended += OnVMGotSuspended;

			_debugeeProcessID = DebugeeProcessIDFromCommandLine();

			var client = provider as Client;
			if (client != null)
				client.Port = ServicePortFromCommandLine();

			provider.StartRefreshingSources(null, null);
		}

		private void SetupDebuggingWindows()
		{
			_windowManager.Add(new ExecutionFlowControlWindow(_debuggingSession));
			_windowManager.Add(new CallStackDisplay(_debuggingSession, _sourceNavigator));
			_windowManager.Add(new ThreadsDisplay(_debuggingSession, new ThreadProvider(_debuggingSession)));
		}


		private void OnVMGotSuspended(Event e)
		{
			//var stackFrames = _debuggingSession.GetMainThread().GetFrames();
			//if (!stackFrames.Any()) return;

			//var topFrame = stackFrames[0];
			//_sourceNavigator.ShowSourceLocation(new Location(topFrame.Location));
		}

		public void OnGUI()
		{
			UnityEngine.Debug.Log ("OnGUI");
//			if (!DebugeeProcessAlive())
//				Application.Quit();

			//if (UnityEngine.Event.current.type == EventType.Layout)
			//    _debuggingSession.Update();

			_windowManager.OnGUI();
		}

		public void FixedUpdate()
		{
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
			var srcsViewPort = new Rect (0, 0, Screen.width * .2f, Screen.height * .7f);
			_sourcesWindow.ViewPort = srcsViewPort;

			var srcViewPort = new Rect (Screen.width * .2f, 0, Screen.width - (Screen.width * .2f), Screen.height * .7f);
			_sourceWindow.ViewPort = srcViewPort;
			
			var consoleTop = srcViewPort.yMax + VerticalSpacing;
			_windowManager.ViewPort = new Rect(0, consoleTop, Screen.width, Screen.height - consoleTop);

			_windowManager.ResetWindows();
		}

		const int VerticalSpacing = 4;

		bool HasArguments() {
			return Environment.GetCommandLineArgs().Count() > 2;
		}

		private int SdbPortFromCommandLine ()
		{
			return ReadIntFromCommandLine (1);
		}
		
		private int ServicePortFromCommandLine ()
		{
			return ReadIntFromCommandLine(2);
		}

		private int DebugeeProcessIDFromCommandLine()
		{
			return ReadIntFromCommandLine(3);
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
//			_debuggingSession.Disconnect();
		}
	}
}
