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

				if (stopWatch.Elapsed > TimeSpan.FromSeconds(DebugeeProgram.DebugMono ? 10000 : 5))
					throw new TimeoutException(msg);

				Thread.Sleep(100);
			}
		}

		public static void WaitFor(Func<bool> condition)
		{
			WaitFor(condition,"No msg");
		}
	}
}
