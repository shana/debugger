namespace CodeEditor.Debugger.Backend
{
	public interface IMethodMirror
	{
		ILocation[] Locations { get; }
	}
}