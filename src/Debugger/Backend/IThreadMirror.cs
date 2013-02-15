using System.Collections.Generic;

namespace Debugger.Backend
{
	public interface IThreadMirror : IWrapper
	{	
		long Id { get; }
		string Name { get; }
		System.Threading.ThreadState ThreadState { get; }
		IList<IStackFrame> GetFrames ();
	}
}
