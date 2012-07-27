using NUnit.Framework;

namespace CodeEditor.Debugger.IntegrationTests
{
	[TestFixture]
	class ExecutingLocationProviderTests : DebuggerTestBase
	{
		[Test]
		public void ReturnsCorrectLocationOnBreakpoint()
		{
			SetupTestWithBreakpoint();

			_vm.OnBreakpoint += e => {
				Assert.AreEqual(9, ExecutingLocationProvider.Location.LineNumber);
				Assert.AreEqual(LocationOfSourceFile, ExecutingLocationProvider.Location.SourceFile);
				Finish ();
			};

			WaitUntilFinished ();
		}
	}
}
