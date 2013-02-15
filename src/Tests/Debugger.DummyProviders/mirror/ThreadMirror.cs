using System;
using System.Collections.Generic;
using Debugger.Backend;

namespace Debugger.DummyProviders
{
	class ThreadMirror : BaseMirror, IThreadMirror
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public IList<IStackFrame> GetFrames ()
		{
			return new StackFrame[0];
		}
		public System.Threading.ThreadState ThreadState { get { return System.Threading.ThreadState.Running; } }
	}
}
