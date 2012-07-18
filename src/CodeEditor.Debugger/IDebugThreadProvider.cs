using System.Collections.Generic;
using CodeEditor.Debugger.Implementation;

namespace CodeEditor.Debugger
{
	public interface IDebugThreadProvider
	{
		IList<DebugThread> Threads { get; }
	}
}