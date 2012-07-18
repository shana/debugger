namespace CodeEditor.Debugger
{
	public interface IDebugLocation
	{
		string File { get; }
		int LineNumber { get; }
	}
}