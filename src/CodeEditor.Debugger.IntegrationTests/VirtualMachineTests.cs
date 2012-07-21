using System;
using System.Diagnostics;
using System.IO;
using Mono.Debugger.Soft;
using NUnit.Framework;
using VirtualMachine = CodeEditor.Debugger.Implementation.VirtualMachine;

namespace CodeEditor.Debugger.IntegrationTests
{
	[TestFixture]
	public class VirtualMachineTests
	{
		private VirtualMachine _vm;
		private bool _finished;

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
			                		Finish();
			                	};

			WaitUntilFinished();
		}

		[Test]
		public void PublishesAssemblyLoadEventOnStartup()
		{
			_vm.OnVMStart += e => _vm.Resume();
			_vm.OnAssemblyLoad += e =>
			                      	{
										Assert.AreEqual(AssemblyName, e.Assembly.GetName().Name);
			                      		Finish();
			                      	};

			WaitUntilFinished();
		}

		[Test]
		public void PublishesVMDeathOnEndOfProgram()
		{
			_vm.OnTypeLoad += e => _vm.Resume();
			_vm.OnAssemblyLoad += e => _vm.Resume();
			_vm.OnVMDeath += e =>
			                 	{
			                 		Assert.NotNull(e);
			                 		Finish();
			                 	};

			WaitUntilFinished();
		}

		[Test]
		public void PublishesTypeLoadEventOnStartup()
		{
			_vm.OnAssemblyLoad += e => _vm.Resume();
			_vm.OnTypeLoad += e =>
			                      	{
										if (ClassName == e.Type.FullName)
				                      		Finish();
			                      		_vm.Resume();
			                      	};
			WaitUntilFinished();
		}

		private void WaitUntilFinished()
		{
			Synchronization.WaitFor(() => _finished, "Waiting for _finished");
			try
			{
				_vm.Exit();
			}catch (ObjectDisposedException)
			{
			}
			Synchronization.WaitFor(() => _vm.Process.HasExited, "Waiting for process to exit");

			foreach(var error in _vm.Errors)
				Console.WriteLine("VM had error: "+error);

			Assert.IsEmpty(_vm.Errors);
		}

		private void Finish()
		{
			_finished = true;
		}

		private static VirtualMachine SetupVirtualMachineRunning(string exe)
		{
			var psi = ProcessStartInfoFor(exe);

			Console.WriteLine((string) psi.FileName);

			//_debugee = new DebugeeProgram();
			var sdb = VirtualMachineManager.Launch(psi, DebuggerOptions);
			var vm = new VirtualMachine(sdb);
			return vm;
		}

		private static LaunchOptions DebuggerOptions
		{
			get { return new LaunchOptions() {/*AgentArgs = "loglevel=2,logfile=c:/as3/sdblog"*/}; }
		}

		public const bool DebugMono = false;

		public static ProcessStartInfo ProcessStartInfoFor(string exe)
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
			if (DebugMono)
				psi.EnvironmentVariables.Add("UNITY_GIVE_CHANCE_TO_ATTACH_DEBUGGER", "1");
			return psi;
		}

		public static string ClassName
		{
			get { return "TestClass"; }
		}

		public static string AssemblyName
		{
			get { return "TestAssembly"; }
		}

		public static string CompileSimpleProgram()
		{
			var csharp = @"
using System;
using System.Net;
using System.Net.Sockets;

class "+ClassName + @"
{
	static void Main()
	{
		Console.WriteLine(""Hello"");
	}
}
";
			var tmp = Path.Combine(Path.GetTempPath(), "source.cs");
			File.WriteAllText(tmp,csharp);
			CSharpCompiler.Compile(Filename, new[] {tmp}, true);
			return Filename;
		}

		public static string Filename
		{
			get { return AssemblyName+".exe"; }
		}
	}
}
