using System;
using Debugger.Backend;

namespace Debugger.DummyProviders
{
	class StackFrame : BaseMirror, IStackFrame
	{
		public IThreadMirror Thread { get; private set; }
		public IMethodMirror Method { get; private set; }
		public int ILOffset { get; private set; }
		public ILocation Location { get; private set; }
	}
}
