using System.Collections.Generic;
using Debugger.Backend;

namespace Debugger
{
	public interface IThreadProvider
	{
		IEnumerable<IThreadMirror> Threads { get; }
	}
}
