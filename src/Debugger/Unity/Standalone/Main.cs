using System;
using System.IO;
using CodeEditor.Composition.Hosting;
using Debugger.Unity.Engine;

namespace Debugger.Unity.Standalone
{
	public class Main
	{
		static MainWindow view;

		public static void Start ()
		{
			var compositionContainer = new CompositionContainer (new DirectoryCatalog (AssemblyPath));
			view = compositionContainer.GetExportedValue<MainWindow> ();
			view.Initialize (compositionContainer);
		}

		public static void OnGUI ()
		{
			view.OnGUI ();
		}

		public static void OnApplicationQuit ()
		{
			view.OnApplicationQuit ();
		}

		static string AssemblyPath
		{
			get { return Path.GetDirectoryName (typeof (Main).Assembly.ManifestModule.FullyQualifiedName); }
		}

		//public static void Update ()
		//{
		//}

		//public static void FixedUpdate ()
		//{

		//}
	}
}
