namespace CodeEditor.Debugger.Backend
{
	public interface ILocation
	{
		string File { get; }
		int LineNumber { get; }
	}
}