namespace Debugger.Backend
{
	public class Wrapper : IWrapper
	{
		protected readonly object obj;

		public Wrapper(object obj)
		{
			this.obj = obj;
		}

		public T Unwrap<T>() where T : class
		{
			return obj as T;
		}
	}
}
