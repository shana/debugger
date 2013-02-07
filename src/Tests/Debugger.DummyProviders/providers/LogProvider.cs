using System;
using CodeEditor.Composition;

namespace Debugger.DummyProviders
{
	[Export(typeof(ILogProvider))]
	public class LogProvider : ILogProvider
	{
		public void Log (string msg)
		{
			throw new NotImplementedException ();
		}
	}
}
