using System;
using System.Collections.Generic;
using CodeEditor.Composition;

namespace Debugger.DummyProviders
{
	[Export(typeof(ISourcesProvider))]
	public class SourcesProvider : ISourcesProvider
	{
		public int Port { get; set; }
		public string Path { get; set; }
		public IList<string> Sources { get; set; }
		public event Action<string> FileChanged;
		public event Action SourcesChanged;
		public void StartRefreshingSources ()
		{
			
		}

		public void StopRefreshingSources ()
		{
			
		}
	}
}
