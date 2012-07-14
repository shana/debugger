using System.IO;
using CodeEditor.Composition.Hosting;

namespace CodeEditor.Debugger
{
	public class Program
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

		static string AssemblyPath
		{
			get { return Path.GetDirectoryName(typeof(Program).Assembly.ManifestModule.FullyQualifiedName); }
		}
	}
}
