using System;
using System.Diagnostics;
using System.Threading;

namespace CodeEditor.Debugger.IntegrationTests
{
	class Synchronization
	{
		public static void WaitFor(Func<bool> condition, string msg)
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();
			while(true)
			{
				if (condition())
					return;

				if (stopWatch.Elapsed > TimeSpan.FromSeconds(IsHumanDebugging() ? 10000 : 5))
					throw new TimeoutException(msg);

				Thread.Sleep(100);
			}
		}

		private static bool IsHumanDebugging()
		{
			return VirtualMachineTests.DebugMono || System.Diagnostics.Debugger.IsAttached;
		}

		public static void WaitFor(Func<bool> condition)
		{
			WaitFor(condition,"No msg");
		}
	}
}
