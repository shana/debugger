using System.Collections.Generic;
using Debugger.Implementation;

namespace Debugger
{
	public interface IThreadProvider
	{
		IList<DebugThread> Threads { get; }
	}
}
