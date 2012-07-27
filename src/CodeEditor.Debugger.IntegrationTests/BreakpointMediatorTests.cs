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
		[Ignore("WIP")]
		public void CanSetBreakpointOnLine ()
		{
			SetupTestWithBreakpoint();

			_vm.OnBreakpoint += e => {
				Assert.AreEqual ("Main", e.Method.FullName);
				Finish ();
			};

			WaitUntilFinished ();
		}
	}
}
