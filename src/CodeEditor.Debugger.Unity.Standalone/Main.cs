using System.IO;
using CodeEditor.Composition.Hosting;

namespace CodeEditor.Debugger.Unity.Standalone
{
	public class Main
	{
		static MainWindow view;

		public static void Start()
		{
			view = new CompositionContainer(new DirectoryCatalog(AssemblyPath)).GetExportedValue<MainWindow>();
		}

		public static void OnGUI()
		{
			view.OnGUI();
		}

		public static void OnApplicationQuit()
		{
			view.OnApplicationQuit();
		}

		static string AssemblyPath
		{
			get { return Path.GetDirectoryName(typeof(Main).Assembly.ManifestModule.FullyQualifiedName); }
		}

		public static void Update()
		{
		}
	}
}
