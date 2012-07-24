using CodeEditor.Debugger.Implementation;

namespace CodeEditor.Debugger
{
	public interface IExecutingLocationProvider
	{
		ILocation Location { get; }
	}
}