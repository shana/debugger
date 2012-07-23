using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Mono.Debugger.Soft;
using NUnit.Framework;
using VirtualMachine = CodeEditor.Debugger.Implementation.VirtualMachine;

namespace CodeEditor.Debugger.IntegrationTests
{
	[TestFixture]
	class VirtualMachineTests : DebuggerTestBase
	{
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
		public void PublishesVMGotSuspendedOnStartup()
		{
			_vm.OnVMGotSuspended += Finish;
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
			_vm.OnVMStart += e => _vm.Resume();
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
			_vm.OnVMStart += e => _vm.Resume();
			_vm.OnAssemblyLoad += e => _vm.Resume();
			_vm.OnTypeLoad += e =>
			                      	{
										if (DebugeeProgramClassName == e.Type.FullName)
				                      		Finish();
			                      		_vm.Resume();
			                      	};
			WaitUntilFinished();
		}

		[Test]
		public void BreakPointOnMainWillHit()
		{
			_vm.OnVMStart += e => _vm.Resume();
			_vm.OnAssemblyLoad += e => _vm.Resume();

			BreakpointEventRequest request = null;
			_vm.OnTypeLoad += e =>
			                  	{
									if (DebugeeProgramClassName == e.Type.FullName)
									{
										var locations = e.Type.GetMethod("Main").Locations;
										Assert.AreEqual(4, locations.Count);
										
										Assert.AreEqual(7, locations[0].LineNumber);
										Assert.AreEqual(8, locations[1].LineNumber);
										Assert.AreEqual(9, locations[2].LineNumber);
										Assert.AreEqual(9, locations[3].LineNumber);

										request = _vm.CreateBreakpointRequest(locations.First());
										request.Enable();
									}
			                  		_vm.Resume();
			                  	};

			_vm.OnBreakpoint += e =>
			                    	{
										Assert.AreEqual("Main", e.Method.Name);
										Assert.AreSame(e.Request,request);
			                    		Finish();
			                    	};
			
			WaitUntilFinished();
		}
	}
}
