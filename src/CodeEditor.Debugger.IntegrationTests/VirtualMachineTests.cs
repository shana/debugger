using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Mono.Debugger.Soft;
using NUnit.Framework;
using VirtualMachine = CodeEditor.Debugger.Implementation.VirtualMachine;

namespace CodeEditor.Debugger.IntegrationTests
{
	[TestFixture]
	public class VirtualMachineTests
	{
		[Test]
		public void CanRunAssembly()
		{
			var csharp = @"
class Test
{
	static void Main()
	{
		System.Console.WriteLine(""Hello"");
	}
}
";
			var exe = Compile(csharp);
			RunWithDebugger(exe);
		}

		private void RunWithDebugger(string exe)
		{
			var psi = new ProcessStartInfo()
			          	{
							Arguments = exe,
			          		CreateNoWindow = true,
			          		UseShellExecute = false,
			          		RedirectStandardOutput = true,
			          		RedirectStandardInput = true,
			          		RedirectStandardError = true,
			          		FileName = Paths.MonoExecutable("bin/cli")
			          	};

			var sdb = VirtualMachineManager.Launch(psi, new LaunchOptions());
			var vm = new VirtualMachine(sdb);

			bool ready = false;
			vm.OnVMStart += delegate
			                          	{
			                          		ready = true;
			                          	};

			WaitFor(() => ready);

			Console.WriteLine("stdout:" + (sdb.Process.HasExited ? sdb.Process.StandardOutput.ReadToEnd() : "process still running"));
		}

		private static void WaitFor(Func<bool> condition)
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			while(true)
			{
				if (condition())
					return;

				if (stopWatch.Elapsed > TimeSpan.FromSeconds(3))
					throw new TimeoutException();

				Thread.Sleep(100);
			}
		}

		private string Compile(string csharp)
		{
			var tmp = Path.Combine(Path.GetTempPath(), "source.cs");
			File.WriteAllText(tmp,csharp);
			CSharpCompiler.Compile("test.exe", new[] {tmp}, true);
			return "test.exe";
		}
	}
}
