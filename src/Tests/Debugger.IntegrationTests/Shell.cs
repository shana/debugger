using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace Debugger.IntegrationTests
{
	internal static class Shell
	{
		public static string Execute(string filename, string arguments)
		{
			var p = StartProcess(filename, arguments);
			var error = p.StandardError.ReadToEnd();
			var output = p.StandardOutput.ReadToEnd();
			p.WaitForExit();
			var allConsoleOutput = error + output;
			Assert.AreEqual(0, p.ExitCode, allConsoleOutput);
			return allConsoleOutput;
		}

		public static Process StartProcess(string filename, string arguments)
		{
			var p = new Process
			{
				StartInfo =
				{
					Arguments = arguments,
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardInput = true,
					RedirectStandardError = true,
					FileName = filename
				}
			};
			p.Start();
			return p;
		}

		public static string CapturingStdout(Action action)
		{
			return Capturing(Console.Out, Console.SetOut, action);
		}

		public static string CapturingStderr(Action action)
		{
			return Capturing(Console.Error, Console.SetError, action);
		}

		private static string Capturing(TextWriter previous, Action<TextWriter> setter, Action action)
		{
			var stdout = new StringWriter();
			setter(stdout);
			try
			{
				action();
			}
			finally
			{
				setter(previous);
			}
			return stdout.ToString();
		}
	}
}
