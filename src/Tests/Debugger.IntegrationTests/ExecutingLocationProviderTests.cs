using NUnit.Framework;

namespace Debugger.IntegrationTests
{
	[TestFixture]
	class ExecutingLocationProviderTests : DebuggerTestBase
	{
		[Test]
		public void ReturnsCorrectLocationOnBreakpoint()
		{
			SetupTestWithBreakpoint(7);

			_vm.OnBreakpoint += e => {
				// the actual line number on the breakpoint is 8, since 7 is the {
				Assert.AreEqual(8, ExecutionProvider.Location.LineNumber);
				Assert.AreEqual(LocationOfSourceFile, ExecutionProvider.Location.SourceFile);
				Finish();
			};

			WaitUntilFinished();
		}
	}
}
