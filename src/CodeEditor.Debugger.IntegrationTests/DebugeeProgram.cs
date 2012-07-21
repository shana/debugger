using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CodeEditor.Debugger.IntegrationTests
{
	class DebugeeProgram
	{
		private readonly Process _process;

		public DebugeeProgram()
		{
			_process = new Process {StartInfo = ProcessStartInfoFor(CompileSimpleProgram())};
			_process.Start();
		}

		public bool HasExited
		{
			get { return _process.HasExited; }
		}

		public string ReadOutput()
		{
			return _process.StandardOutput.ReadToEnd();
		}

		public void InformDebuggerAttached()
		{
			new TcpClient().Connect(new IPEndPoint(IPAddress.Loopback, 4321));
		}

		public const bool DebugMono = false;

		public static ProcessStartInfo ProcessStartInfoFor(string exe)
		{
			var psi = new ProcessStartInfo()
			          	{
			          		Arguments = exe,
			          		//WorkingDirectory = "c:\\as3",
			          		CreateNoWindow = true,
			          		UseShellExecute = false,
			          		RedirectStandardOutput = true,
			          		RedirectStandardInput = true,
			          		RedirectStandardError = true,
			          		FileName = Paths.MonoExecutable("bin/cli")
			          	};
			if (DebugMono)
				psi.EnvironmentVariables.Add("UNITY_GIVE_CHANCE_TO_ATTACH_DEBUGGER", "1");
			return psi;
		}

		public static string CompileSimpleProgram()
		{
			var csharp = @"
using System;
using System.Net;
using System.Net.Sockets;

class Test
{
	static void Main()
	{
		Console.WriteLine(""MainStarting"");
		var listener = new TcpListener(IPAddress.Loopback, 4321);
		listener.Start();
		listener.AcceptTcpClient();
		Console.WriteLine(""ohai"");
	}
}

class AnotherClass
{
	public static void Hello()
	{
	}
}
";
			var tmp = Path.Combine(Path.GetTempPath(), "source.cs");
			File.WriteAllText(tmp,csharp);
			CSharpCompiler.Compile("test.exe", new[] {tmp}, true);
			return "test.exe";
		}
	}
}
