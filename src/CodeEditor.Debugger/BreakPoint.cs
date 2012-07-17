namespace CodeEditor.Debugger
{
	public interface IBreakPoint
	{
		string File { get; }
		int LineNumber { get; }
	}

	class BreakPoint : IBreakPoint
	{
		public string File { get; private set; }
		public int LineNumber { get; private set; }

		public BreakPoint(string fileName, int lineNumber)
		{
			File = fileName;
			LineNumber = lineNumber;
		}
	}
}