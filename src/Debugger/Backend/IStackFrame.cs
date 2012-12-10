namespace Debugger.Backend
{
	public interface IStackFrame : IWrapper
	{
		IThreadMirror Thread { get; }
		IMethodMirror Method { get; }
		int ILOffset { get; }
		ILocation Location { get; }
	}
}
