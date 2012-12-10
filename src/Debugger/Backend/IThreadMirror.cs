namespace Debugger.Backend
{
	public interface IThreadMirror : IWrapper
	{	
		long Id { get; }
		string Name { get; }
		IStackFrame[] GetFrames ();
	}
}
