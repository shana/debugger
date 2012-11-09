using MDS=Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class SdbThreadMirror : Wrapper, IThreadMirror
	{
		public long Id { get { return Unwrap<SdbThreadMirror>().Id; } }
		public string Name { get { return Unwrap<SdbThreadMirror>().Name; } }


		public SdbThreadMirror(MDS.ThreadMirror threadMirror) : base(threadMirror)
		{
		}
	}
}
