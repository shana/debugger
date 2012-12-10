using NUnit.Framework;

namespace Debugger.IntegrationTests
{
	[TestFixture]
	class BreakpointMediatorTests : DebuggerTestBase
	{
		[Test]
		public void CanSetBreakpointOnLine()
		{
			SetupTestWithBreakpoint(7);

			//_vm.OnBreakpoint += e => {
			//    Assert.AreEqual("Void TestClass:Main ()", e.Method.FullName);
			//    Finish();
			//};

			WaitUntilFinished();
		}
	}
}
