using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CodeEditor.Composition;
using CodeEditor.Composition.Hosting;
using NUnit.Framework;

namespace CodeEditor.Debugger.IntegrationTests
{
	[TestFixture]
	public class Tests
	{
		private const string DebuggerArguments = "--debugger-agent=transport=dt_socket,defer=y,suspend=y";

		[Test]
		public void CanRunAssembly()
		{
			var csharp = @"
class Test
{
	static void Main()
	{
		System.Console.WriteLine(""Hello"");
	}
}
";
			var exe = Compile(csharp);
			RunWithDebugger(exe);
		}

		[Export(typeof(ILogProvider))]
		class TestLogProvider : ILogProvider
		{
			public void Log(string msg)
			{
				Console.WriteLine(msg);
			}
		}

		private void RunWithDebugger(string exe)
		{
			var process = Shell.StartProcess(Paths.MonoExecutable("bin/cli"), exe + " " + DebuggerArguments);
			Console.WriteLine("Assemblypath: "+AssemblyPath);
			var compositionContainer = new CompositionContainer(typeof(IDebuggerSession).Assembly, GetType().Assembly);
			compositionContainer.GetExportedValue<IDebuggerSessionAssembler>().Assemble();
			var session = compositionContainer.GetExportedValue<IDebuggerSession>();

			bool ready = false;
			session.VMGotSuspended += delegate
			                          	{
			                          		session.SafeResume();
			                          		ready = true;
			                          	};
			//session.Start(DebuggerPortFor(process));

			var stopWatch = new Stopwatch();
			stopWatch.Start();
			while(!ready && stopWatch.Elapsed < TimeSpan.FromSeconds(3))
			{
				session.Update();
				Thread.Sleep(100);
			}
			Console.WriteLine("stdout:" +process.StandardOutput.ReadToEnd());
		}

		static string AssemblyPath
		{
			get { return Path.GetDirectoryName(typeof(Tests).Assembly.ManifestModule.FullyQualifiedName); }
		}

		private string Compile(string csharp)
		{
			var tmp = Path.Combine(Path.GetTempPath(), "source.cs");
			File.WriteAllText(tmp,csharp);
			CSharpCompiler.Compile("test.exe", new[] {tmp}, true);
			return "test.exe";
		}
	}
}
