namespace Debugger.Backend
{
	public interface ILocation : IWrapper
	{
		string File { get; }
		int LineNumber { get; }
	}
}
