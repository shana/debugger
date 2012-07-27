namespace CodeEditor.Debugger
{
	public interface ILocation
	{
		int LineNumber { get; }
		string SourceFile { get; }
	}
}