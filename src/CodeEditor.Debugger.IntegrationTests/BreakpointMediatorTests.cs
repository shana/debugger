using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeEditor.Debugger.Implementation;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Debugger.IntegrationTests
{
	[TestFixture]
	class BreakpointMediatorTests : DebuggerTestBase
	{
		[Test]
		public void CanSetBreakpointOnLine()
		{
			SetupTestWithBreakpoint(7);

			_vm.OnBreakpoint += e => {
				Assert.AreEqual("Void TestClass:Main ()", e.Method.FullName);
				Finish();
			};

			WaitUntilFinished();
		}
	}
}
