using System.Collections.Generic;

namespace Debugger
{
	public interface IThreadProvider
	{
		IList<DebugThread> Threads { get; }
	}
}
