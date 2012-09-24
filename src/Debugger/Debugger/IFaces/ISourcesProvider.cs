using System;
using System.Collections.Generic;

namespace CodeEditor.Debugger
{
	public interface ISourcesProvider
	{
		IList<string> Sources { get; }

		void StartRefreshingSources(EventHandler callback, object state);
		void StopRefreshingSources();
		void Stop();
	}
}
