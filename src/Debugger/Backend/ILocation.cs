namespace Debugger.Backend
{
	public interface ILocation : IWrapper
	{
		string SourceFile { get; }
		int LineNumber { get; }
	}
}
