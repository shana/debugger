using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Debugger.Backend;
using MDS=Mono.Debugger.Soft;
using NUnit.Framework;

namespace Debugger.IntegrationTests
{
	internal class DebuggerTestBase
	{
		public const bool DebugMono = true;
		protected IVirtualMachine _vm;
		private bool _finished;
		private IBreakpointProvider _breakpointProvider;
		protected IExecutionProvider ExecutionProvider;

		private static string DebuggerOptions = "loglevel=2,logfile=c:/as3/sdblog";

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
			try
			{
				Synchronization.WaitFor (() => _finished, "Waiting for _finished");
			}
			catch (TimeoutException)
			{
				// the test timed out, meaning one of the asserts failed and Finish() didn't run
			}
			try
			{
//				_vm.Exit ();
			}
			catch (Exception ex)
			{
				// this should never happen
				Console.WriteLine ("Exception thrown while exiting the VM");
				Console.WriteLine (ex);
			}
			try
			{
//				Synchronization.WaitFor (() => _vm.Process.HasExited, "Waiting for process to exit");
			}
			catch (TimeoutException)
			{
//				_vm.Process.Kill ();
				Console.WriteLine ("VM process did not exit cleanly");
			}

			//foreach (var error in _vm.Errors)
			//    Console.WriteLine ("VM had error: "+error);

			//Assert.IsEmpty (_vm.Errors);
		}

		protected void Finish ()
		{
			_finished = true;
		}

		private static IVirtualMachine SetupVirtualMachineRunning (string exe)
		{
			var psi = ProcessStartInfoFor (exe);

			Console.WriteLine ((string) psi.FileName);

			var sdb = DebuggerSession.Launch ((ProcessStartInfo) psi, DebuggerOptions);
//			var vm = new VirtualMachine (sdb);
			return sdb.VM;
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
				if (!psi.EnvironmentVariables.ContainsKey("UNITY_GIVE_CHANCE_TO_ATTACH_DEBUGGER"))
					psi.EnvironmentVariables.Add("UNITY_GIVE_CHANCE_TO_ATTACH_DEBUGGER", "1");
				else
					psi.EnvironmentVariables["UNITY_GIVE_CHANCE_TO_ATTACH_DEBUGGER"] = "1";
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

		protected void SetupTestWithBreakpoint(int line)
		{
			_breakpointProvider = new BreakpointProvider();
			_breakpointProvider.ToggleBreakpointAt(LocationOfSourceFile, line);
			new BreakpointMediator(_vm, _breakpointProvider);
			ExecutionProvider = new ExecutionProvider(_vm);
		}
	}
}
