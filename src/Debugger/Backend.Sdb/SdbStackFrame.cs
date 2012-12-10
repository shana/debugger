namespace Debugger.Backend.Sdb
{
	public class SdbStackFrame : Wrapper, IStackFrame
	{
		public SdbStackFrame(Mono.Debugger.Soft.StackFrame obj) : base(obj)
		{
		}

		public IThreadMirror Thread { get { return new SdbThreadMirror(Unwrap<Mono.Debugger.Soft.StackFrame>().Thread); } }

		public IMethodMirror Method { get { return new SdbMethodMirror(Unwrap<Mono.Debugger.Soft.StackFrame>().Method); } }

		public int ILOffset { get { return Unwrap<Mono.Debugger.Soft.StackFrame>().ILOffset; } }

		public ILocation Location { get { return new SdbLocation(Unwrap<Mono.Debugger.Soft.StackFrame>().Location); } }
	}
}