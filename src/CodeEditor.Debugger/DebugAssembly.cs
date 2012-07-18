using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger
{
	class DebugAssembly : IDebugAssembly
	{
		public DebugAssembly(AssemblyMirror assemblyMirror)
		{
			Mirror = assemblyMirror;
		}

		public AssemblyMirror Mirror { get; private set; }
	}

	public interface IDebugAssembly
	{
	}
}
