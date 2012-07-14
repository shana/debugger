using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace CodeEditor.Debugger.UnityEditor
{
	public static class DebuggerMenuItems
	{
		[MenuItem("Debug/Start")]
		public static void StartDebugger()
		{
			Process.Start(DebuggerExe(), DebuggerConnectionPort().ToString());
		}

		private static string DebuggerExe()
		{
			return Path.GetFullPath("CodeEditor/Debugger/Debugger.exe");
		}

		private static int DebuggerConnectionPort()
		{
			return 56000 + (Process.GetCurrentProcess().Id % 1000);
		}
	}
}
