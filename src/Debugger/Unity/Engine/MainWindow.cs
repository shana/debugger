using System;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Unity.Engine;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export]
	public class MainWindow
	{
		private readonly SourcesWindow sourcesWindow;
		private readonly SourceWindow sourceWindow;
		private readonly LogWindow log;
		private readonly CallStackDisplay callStackDisplay;
		private readonly ExecutionWindow executionWindow;
		private readonly BreakpointsWindow breakpointsWindow;

		private readonly IDebuggerSession session;
		private readonly ITypeProvider typeProvider;
		private readonly DebuggerWindowManager windowManager;

		public static GUISkin skin;

		[ImportingConstructor]
		public MainWindow (
			IDebuggerSession session,
			ITypeProvider typeProvider,
			SourcesWindow sourcesWindow,
			SourceWindow sourceWindow,
			LogWindow log,
			CallStackDisplay callStackDisplay,
			ExecutionWindow executionWindow,
			BreakpointsWindow breakpointsWindow,
			DebuggerWindowManager windowManager
		)
		{
			this.log = log;
			this.callStackDisplay = callStackDisplay;
			this.executionWindow = executionWindow;
			this.breakpointsWindow = breakpointsWindow;
			this.sourcesWindow = sourcesWindow;
			this.sourceWindow = sourceWindow;
			this.session = session;
			this.typeProvider = typeProvider;
			this.windowManager = windowManager;

			Initialize ();
		}

		void Initialize ()
		{

			if (HasArguments ())
				session.Port = SdbPortFromCommandLine ();

			else
			{
				var f = new StreamReader (File.Open (@"C:\debug.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
				var str = f.ReadLine ();
				f.Close ();
				session.Port = int.Parse (str.Substring ("Listening on 0.0.0.0:".Length, 5));
			}

			log.Log ("Connecting to " + session.Port);

			Camera.main.backgroundColor = new Color (0.125f, 0.125f, 0.125f, 0);
			Application.runInBackground = true;

			AdjustLayout ();

//			if (!HasArguments ())
//				return;

			session.TraceCallback += s => Debug.Log (s);
			typeProvider.BasePath = ProjectPathFromCommandLine ();


			sourcesWindow.StartRefreshing ();

			session.Start ();

			skin = Resources.Load("Skins/Generated/DarkSkin", typeof(GUISkin)) as GUISkin;
			Debug.Log ("skin " + skin);
		}

		public void OnGUI ()
		{
			GUI.skin = skin;

			windowManager.OnGUI ();
		}

		private void AdjustLayout ()
		{
			var srcsViewPort = new Rect (0, 0, Screen.width * .2f, Screen.height * .7f);
			sourcesWindow.ViewPort = srcsViewPort;

			var srcViewPort = new Rect (Screen.width * .2f, 0, Screen.width * .7f, Screen.height * .7f);
			sourceWindow.ViewPort = srcViewPort;
			sourceWindow.ExpandWidth = true;

			var consoleTop = srcViewPort.yMax + VerticalSpacing;
			windowManager.ViewPort = new Rect (0, consoleTop, Screen.width, Screen.height - consoleTop);

			windowManager.ResetWindows ();
		}

		const int VerticalSpacing = 4;

		bool HasArguments ()
		{
			return Environment.GetCommandLineArgs ().Count () > 2;
		}

		int SdbPortFromCommandLine ()
		{
			return ReadIntFromCommandLine (1);
		}

		string ProjectPathFromCommandLine ()
		{
			return Environment.GetCommandLineArgs ()[3];
		}

		static int ReadIntFromCommandLine (int index)
		{
			var args = Environment.GetCommandLineArgs ();
			return int.Parse (args[index]);
		}

		void Trace (string format, params object[] args)
		{
			var text = string.Format (format, args);
			Console.WriteLine (text);
			log.WriteLine (text);
		}

		public void OnApplicationQuit ()
		{
			if (session.Active)
				session.Stop ();
		}
	}
}
