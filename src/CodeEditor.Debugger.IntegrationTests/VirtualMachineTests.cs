using System;
using System.Collections.Specialized;
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

		private const bool DebugMono = false;

		[SetUp]
		public void SetUp()
		{
			_vm = SetupVirtualMachineRunning(CompileSimpleProgram());
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
			_vm.OnTypeLoad += e => _vm.Resume();
			_vm.OnVMDeath += e =>
			                 	{
			                 		Assert.NotNull(e);
			                 		Finish();
			                 	};

			WaitUntilFinished();
		}

		[Test]
		[Ignore("WIP")]
		public void PublishesTypeLoadEventOnStartup()
		{
			_vm.OnVMStart += e => _vm.Resume();
			_vm.OnTypeLoad += e =>
			                      	{
										Assert.AreEqual("Test", e.Type.FullName);
			                      		Finish();
			                      	};
			WaitUntilFinished();
		}

		private void WaitUntilFinished()
		{
			WaitFor(() => _finished, "Waiting for _finished");
			try
			{
				_vm.Exit();
			}catch (ObjectDisposedException)
			{
			}
			WaitFor(() => _vm.Process.HasExited, "Waiting for process to exit");
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
							WorkingDirectory = "c:\\as3",
			          		CreateNoWindow = true,
			          		UseShellExecute = false,
			          		RedirectStandardOutput = true,
			          		RedirectStandardInput = true,
			          		RedirectStandardError = true,
			          		FileName = Paths.MonoExecutable("bin/cli")
			          	};
			if (DebugMono)
				psi.EnvironmentVariables.Add("UNITY_GIVE_CHANCE_TO_ATTACH_DEBUGGER","1");

			Console.WriteLine(psi.FileName);
			var sdb = VirtualMachineManager.Launch(psi, new LaunchOptions() {AgentArgs = "loglevel=1,logfile=sdblog"});
			var vm = new VirtualMachine(sdb);
			return vm;
		}

		private static void WaitFor(Func<bool> condition, string msg)
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			while(true)
			{
				if (condition())
					return;

				if (stopWatch.Elapsed > TimeSpan.FromSeconds(DebugMono ? 10000 : 5))
					throw new TimeoutException(msg);

				Thread.Sleep(100);
			}
		}

		private void WaitFor(Func<bool> condition)
		{
			WaitFor(condition,"No msg");
		}

		private string CompileSimpleProgram()
		{
			var csharp = @"
class Test
{
	static void Main()
	{
		System.Console.WriteLine(""Hello"");
		AnotherClass.Hello();
	}
}

class AnotherClass
{
	public static void Hello()
	{
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
