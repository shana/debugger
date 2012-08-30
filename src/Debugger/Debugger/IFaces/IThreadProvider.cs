using System.Collections.Generic;
using CodeEditor.Debugger.Implementation;

namespace CodeEditor.Debugger
{
	public interface IThreadProvider
	{
		IList<DebugThread> Threads { get; }
	}
}