using System;
using System.IO;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Composition.Hosting;
using Debugger.Unity.Engine;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export]
	public class MainWindow
	{
		private SourcesWindow sourcesWindow;
		private SourceWindow sourceWindow;
		private LogWindow log;

		private readonly IDebuggerSession session;
		private readonly ITypeProvider typeProvider;
		private readonly DebuggerWindowManager windowManager;

		[ImportingConstructor]
		public MainWindow (
			IDebuggerSession session,
			ITypeProvider typeProvider,
			DebuggerWindowManager windowManager
		)
		{
			this.session = session;
			this.typeProvider = typeProvider;
			this.windowManager = windowManager;
		}

		public void Initialize (CompositionContainer composition)
		{
			sourcesWindow = composition.GetExportedValue<SourcesWindow> ();
			sourceWindow = composition.GetExportedValue<SourceWindow> ();
			windowManager.Add (sourcesWindow);
			windowManager.Add (sourceWindow);
			log = composition.GetExportedValue<LogWindow> ();
//			windowManager.Add (log);
			windowManager.Add (composition.GetExportedValue<CallstackWindow> ());
			windowManager.Add (composition.GetExportedValue<LocalsWindow> ());
			windowManager.Add (composition.GetExportedValue<BreakpointsWindow> ());
			windowManager.Add (composition.GetExportedValue<ExecutionWindow> ());

			if (HasArguments ())
				session.Port = SdbPortFromCommandLine ();

			//else
			//{
			//    var f = new StreamReader (File.Open (@"C:\debug.log", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
			//    var str = f.ReadLine ();
			//    f.Close ();
			//    session.Port = int.Parse (str.Substring ("Listening on 0.0.0.0:".Length, 5));
			//}

			log.WriteLine ("Connecting to " + session.Port);

			Camera.main.backgroundColor = new Color (0.125f, 0.125f, 0.125f, 0);
			Application.runInBackground = true;

			AdjustLayout ();

//			if (!HasArguments ())
//				return;

			session.TraceCallback += s => Trace (s);
			typeProvider.BasePath = ProjectPathFromCommandLine ();


			sourcesWindow.StartRefreshing ();

			session.Start ();
		}

		public void OnGUI ()
		{
			GUI.skin = Styles.skin;

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
