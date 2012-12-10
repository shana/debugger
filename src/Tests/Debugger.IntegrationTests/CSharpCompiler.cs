using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Debugger.IntegrationTests
{
	public static class CSharpCompiler
	{
		// Returns output path of assembly
		public static string CompileAssemblyFromSourceDir(string fixtureName, string assemblyName, string sourceDir, bool generateExecutable = false, params string[] references)
		{
			var sourceFiles = SourceFilesIn(sourceDir);
			var cacheInfo = new CachedAssemblyInfo(sourceFiles, references);

			var cacheKey = AssemblyCache.GetCacheKey(fixtureName, assemblyName, cacheInfo);
			var outputAssembly = cacheKey + (generateExecutable ? ".exe" : ".dll");

			if (AssemblyCache.CheckCachedAssembly(fixtureName, assemblyName, cacheInfo))
			{
				var cachedCompilerOutput = File.ReadAllText(cacheKey + ".compilerOutput");
				Console.WriteLine("// Using cached assembly '{0}'. Original compiler output follows:", Path.GetFileName(Path.GetDirectoryName(cacheKey)));
				Console.WriteLine(cachedCompilerOutput);
				return outputAssembly;
			}

			var compilerOutput = Compile(outputAssembly, sourceFiles, generateExecutable, references);

			File.WriteAllText(cacheKey + ".compilerOutput", compilerOutput);
			Console.WriteLine(compilerOutput);

			AssemblyCache.SaveCachedAssembly(fixtureName, assemblyName, cacheInfo);

			return outputAssembly;
		}

		public static string[] SourceFilesIn(string sourceDir)
		{
			return Directory.GetFiles(sourceDir, "*.cs", SearchOption.AllDirectories);
		}

		public static string Compile(string outputAssembly, string[] sourceFiles, bool generateExecutable = false, params string[] references)
		{
			var args = new List<string>
			{
				"-out:\"" + outputAssembly + "\"",
				"-t:" + (generateExecutable ? "exe" : "library"),
				"-debug+",
				"-define:TRACE",
				"-r:System.dll",
				"-r:System.Core.dll"
			};
			args.AddRange(references.Select(r => "-r:\"" + r + "\""));
			args.AddRange(sourceFiles);

			return Shell.Execute(Paths.MonoExecutable("bin/smcs"), string.Join(" ", args));
		}
	}
}
