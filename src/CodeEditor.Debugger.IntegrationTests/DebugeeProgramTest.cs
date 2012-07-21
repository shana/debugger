using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NUnit.Framework;

namespace CodeEditor.Debugger.IntegrationTests
{
	[TestFixture]
	public class DebugeeProgramTests
	{
		[Test]
		public void DebuggerTestProgramWorks()
		{
			var program = new DebugeeProgram();

			program.InformDebuggerAttached();

			Synchronization.WaitFor(() => program.HasExited);
			Console.WriteLine(program.ReadOutput());
		}
	}
}