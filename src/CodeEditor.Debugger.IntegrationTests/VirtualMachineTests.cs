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
		private VirtualMachine _vm;
		private bool _finished = false;

		[SetUp]
		public void SetUp()
		{
			_vm = SetupVirtualMachineRunning(CompileSimpleProgram());
		}

		//[TearDown]
		public void TearDown()
		{
			var process = _vm.Process;
			_vm.Exit();
			
			Console.WriteLine("stdout:" + (process.HasExited ? process.StandardOutput.ReadToEnd() : "process still running"));			
		}

		[Test]
		public void PublishesVMStartEventOnStartup()
		{
			_vm.OnVMStart += e =>
			                	{
			                		Assert.IsNotNull(e);
			                		_vm.Resume();
									Finish(); 
								};

			WaitUntilFinished();
		}

		[Test]
		public void PublishesVMDeathOnEndOfProgram()
		{
			_vm.OnVMStart += e => _vm.Resume();
			_vm.OnVMDeath += e =>
			                 	{
			                 		Assert.NotNull(e);
			                 		_vm.Resume();
			                 		Finish();
			                 	};
		}

		private void WaitUntilFinished()
		{
			WaitFor(() => _finished);
		}

		private void Finish()
		{
			_finished = true;
		}

		private static VirtualMachine SetupVirtualMachineRunning(string exe)
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
			return vm;
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

		private string CompileSimpleProgram()
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
			var tmp = Path.Combine(Path.GetTempPath(), "source.cs");
			File.WriteAllText(tmp,csharp);
			CSharpCompiler.Compile("test.exe", new[] {tmp}, true);
			return "test.exe";
		}
	}
}
