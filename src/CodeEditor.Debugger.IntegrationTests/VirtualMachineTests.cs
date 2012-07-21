using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
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
			_vm = SetupVirtualMachineRunning(DebugeeProgram.CompileSimpleProgram());
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
			Synchronization.WaitFor(() => _finished, "Waiting for _finished");
			try
			{
				_vm.Exit();
			}catch (ObjectDisposedException)
			{
			}
			Synchronization.WaitFor(() => _vm.Process.HasExited, "Waiting for process to exit");
		}

		private void Finish()
		{
			_finished = true;
		}

		private static VirtualMachine SetupVirtualMachineRunning(string exe)
		{
			var psi = DebugeeProgram.ProcessStartInfoFor(exe);

			Console.WriteLine(psi.FileName);
			var sdb = VirtualMachineManager.Launch(psi, new LaunchOptions() {AgentArgs = "loglevel=4,logfile=sdblog"});
			var vm = new VirtualMachine(sdb);
			return vm;
		}
	}
}
