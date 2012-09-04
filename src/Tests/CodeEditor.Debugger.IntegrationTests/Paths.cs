using System;
using System.IO;

namespace CodeEditor.Debugger.IntegrationTests
{
	public static class Paths
	{
		public static string ProjectPath(string path)
		{
			return Path.GetFullPath(Path.Combine(ProjectDirectory, path));
		}

		private static string CalculateProjectDirectory()
		{
			var assembly = new Uri(typeof(Paths).Assembly.CodeBase).AbsolutePath;
			return Path.Combine(Path.GetDirectoryName(assembly), "../../..");
		}

		static readonly string ProjectDirectory = CalculateProjectDirectory();

		public static string ExecutablePath(string path)
		{
			return ExecutablePath(path, ".exe");
		}

		private static string ExecutablePath(string path, string windowsExtension)
		{
			return FixExecutableExtension(ProjectPath(path), windowsExtension);
		}

		public static string MonoExecutable(string path)
		{
			return FixExecutableExtension(MonoPath(path), ".bat");
		}

		private static string FixExecutableExtension(string result, string windowsExtension)
		{
			if (IsWindows()) result += windowsExtension;
			return result;
		}

		private static bool IsWindows()
		{
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32Windows:
				case PlatformID.Win32NT:
					return true;
			}
			return false;
		}

		public static string MonoPath(string path)
		{
			return Path.Combine(ProjectPath("../../../../External/Mono/builds/monodistribution"), path);
		}

		public static string PrepareFileName(string file)
		{
			return "\"" + file + "\"";
		}
	}
}
