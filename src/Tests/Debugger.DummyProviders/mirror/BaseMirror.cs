using System;
using Debugger.Backend;

namespace Debugger.DummyProviders
{
	public class BaseMirror : IWrapper
	{
		public T Unwrap<T> () where T : class
		{
			throw new NotImplementedException ();
		}
	}
}
