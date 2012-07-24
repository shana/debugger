using System;
using System.Diagnostics;
using System.IO;
using CodeEditor.Debugger.Implementation;
using Mono.Debugger.Soft;
using NUnit.Framework;
using VirtualMachine = CodeEditor.Debugger.Implementation.VirtualMachine;

namespace CodeEditor.Debugger.IntegrationTests
{
	internal class DebuggerTestBase
	{
		public const bool DebugMono = false;
		protected VirtualMachine _vm;
		private bool _finished;
		private BreakpointProvider _breakpointProvider;
		protected IExecutingLocationProvider ExecutingLocationProvider;

		private static LaunchOptions DebuggerOptions
		{
			get { return new LaunchOptions () { AgentArgs = "loglevel=2,logfile=c:/as3/sdblog" }; }
		}

		public static string DebugeeProgramClassName
		{
			get { return "TestClass"; }
		}

		public static string AssemblyName
		{
			get { return "TestAssembly"; }
		}

		public static string Filename
		{
			get { return AssemblyName+".exe"; }
		}

		[SetUp]
		public void SetUp()
		{
			_vm = SetupVirtualMachineRunning (CompileSimpleProgram ());
		}

		protected void WaitUntilFinished ()
		{
			Synchronization.WaitFor (() => _finished, "Waiting for _finished");
			try
			{
				_vm.Exit ();
			}
			catch (ObjectDisposedException)
			{
			}
			Synchronization.WaitFor (() => _vm.Process.HasExited, "Waiting for process to exit");

			foreach (var error in _vm.Errors)
				Console.WriteLine ("VM had error: "+error);

			Assert.IsEmpty (_vm.Errors);
		}

		protected void Finish ()
		{
			_finished = true;
		}

		private static VirtualMachine SetupVirtualMachineRunning (string exe)
		{
			var psi = ProcessStartInfoFor (exe);

			Console.WriteLine ((string) psi.FileName);

			var sdb = VirtualMachineManager.Launch ((ProcessStartInfo) psi, DebuggerOptions);
			var vm = new VirtualMachine (sdb);
			return vm;
		}

		public static ProcessStartInfo ProcessStartInfoFor (string exe)
		{
			var psi = new ProcessStartInfo ()
						  {
							  Arguments = exe,
							  CreateNoWindow = true,
							  UseShellExecute = false,
							  RedirectStandardOutput = true,
							  RedirectStandardInput = true,
							  RedirectStandardError = true,
							  FileName = Paths.MonoExecutable ("bin/cli")
						  };

			if (DebugMono)
				psi.EnvironmentVariables.Add ("UNITY_GIVE_CHANCE_TO_ATTACH_DEBUGGER", "1");
			return psi;
		}

		public static string CompileSimpleProgram ()
		{
			var csharp = @"
using System;

class "+DebugeeProgramClassName + @"
{
	static void Main()
	{
		Console.WriteLine(""Hello"");
	}
}
";
			var tmp = LocationOfSourceFile;
			File.WriteAllText (tmp,csharp);
			CSharpCompiler.Compile (Filename, new[] {tmp}, true);
			return Filename;
		}

		protected static string LocationOfSourceFile
		{
			get { return Path.Combine (Path.GetTempPath (), SourceFileName); }
		}

		public static string SourceFileName
		{
			get { return "source.cs"; }
		}

		protected void SetupTestWithBreakpoint()
		{
			_vm.OnVMStart += e => _vm.Resume();
			_vm.OnTypeLoad += e => _vm.Resume();
			_vm.OnAssemblyLoad += e => _vm.Resume();

			_breakpointProvider = new BreakpointProvider();
			_breakpointProvider.ToggleBreakPointAt(LocationOfSourceFile, 9);
			new BreakpointMediator(_vm, _breakpointProvider);
			ExecutingLocationProvider = new ExecutingLocationProvider(_vm);
		}
	}
}