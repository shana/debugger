namespace CodeEditor.Debugger
{
	public interface IBreakpointEventRequestFactory
	{
		IDebugBreakpointEventRequest Create(IDebugLocation location);
	}
}