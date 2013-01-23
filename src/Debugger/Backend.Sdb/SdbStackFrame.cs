using MDS=Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbStackFrame : Wrapper, IStackFrame
	{
		public SdbStackFrame (MDS.StackFrame obj)
			: base (obj)
		{
		}

		public IThreadMirror Thread
		{
			get { return Cache.Lookup<SdbThreadMirror> (Unwrap<Mono.Debugger.Soft.StackFrame> ().Thread); }
		}

		public IMethodMirror Method
		{
			get { return Cache.Lookup<SdbMethodMirror> (Unwrap<Mono.Debugger.Soft.StackFrame> ().Method); }
		}

		public int ILOffset { get { return Unwrap<Mono.Debugger.Soft.StackFrame> ().ILOffset; } }

		public ILocation Location
		{
			get { return Cache.Lookup<SdbLocation> (Unwrap<Mono.Debugger.Soft.StackFrame> ().Location); }
		}
	}
}
