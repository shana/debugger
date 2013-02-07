using System.Collections.Generic;
using Debugger.Backend;

namespace Debugger
{
	public interface IThreadProvider
	{
		IList<IThreadMirror> Threads { get; }
	}
}
