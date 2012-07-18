namespace CodeEditor.Debugger
{
	public interface IBreakPoint
	{
		string File { get; }
		int LineNumber { get; }
	}
}