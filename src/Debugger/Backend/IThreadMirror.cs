using System.Collections.Generic;

namespace Debugger.Backend
{
	public interface IThreadMirror : IWrapper
	{	
		long Id { get; }
		string Name { get; }
		IList<IStackFrame> GetFrames ();
	}
}
