using CodeEditor.Debugger.Backend;

namespace CodeEditor.Debugger
{
	public interface IBreakpointEventRequestFactory
	{
		IBreakpointEventRequest Create(ILocation location);
	}
}