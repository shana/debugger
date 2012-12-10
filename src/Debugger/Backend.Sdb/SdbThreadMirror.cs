using MDS=Mono.Debugger.Soft;
using System.Linq;

namespace Debugger.Backend.Sdb
{
	public class SdbThreadMirror : Wrapper, IThreadMirror
	{
		private MDS.ThreadMirror _threadMirror { get { return _obj as MDS.ThreadMirror; } }
		private IStackFrame[] _frames;

		public long Id { get { return Unwrap<MDS.ThreadMirror>().Id; } }
		public string Name { get { return Unwrap<MDS.ThreadMirror>().Name; } }

		public SdbThreadMirror(MDS.ThreadMirror threadMirror) : base(threadMirror)
		{
			_frames = threadMirror.GetFrames().Select(x => new SdbStackFrame(x)).ToArray();
		}

		public MDS.StackFrame[] Frames { get { return Unwrap<MDS.ThreadMirror>().GetFrames(); } }

		public IStackFrame[] GetFrames()
		{
			return _frames;
		}

	}
}
