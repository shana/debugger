namespace Debugger.Backend
{
	public interface IVariable : IWrapper
	{
		string Name { get; }
		ITypeMirror Type { get; }
		bool IsArgument { get; }
		object Value { get; set; }
	}
}
