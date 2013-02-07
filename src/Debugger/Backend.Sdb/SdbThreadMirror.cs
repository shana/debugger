using System.Collections.Generic;
using MDS = Mono.Debugger.Soft;
using System.Linq;

namespace Debugger.Backend.Sdb
{
	public class SdbThreadMirror : Wrapper, IThreadMirror
	{
		private MDS.ThreadMirror threadMirror { get { return obj as MDS.ThreadMirror; } }
		private IStackFrame[] frames;

		public long Id { get { return Unwrap<MDS.ThreadMirror> ().Id; } }
		public string Name { get { return Unwrap<MDS.ThreadMirror> ().Name; } }

		public SdbThreadMirror (MDS.ThreadMirror threadMirror)
			: base (threadMirror)
		{
		}

		public IList<IStackFrame> GetFrames ()
		{
			if (frames == null)
				frames = threadMirror.GetFrames ().Select (x => new SdbStackFrame (x)).ToArray ();
			return frames;
		}

		public override bool Equals (object obj)
		{
			var right = obj as SdbThreadMirror;
			if (right == null)
				return false;
			return this.GetHashCode () == right.GetHashCode ();
		}

		public override int GetHashCode ()
		{
			return (int)Id;
		}
	}
}
