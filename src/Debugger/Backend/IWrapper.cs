namespace Debugger.Backend
{
	public interface IWrapper
	{
		T Unwrap<T>() where T : class;
	}
}
