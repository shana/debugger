using System.Collections.Generic;
using CodeEditor.Composition;
using Debugger.Backend;

namespace Debugger.DummyProviders
{
	[Export(typeof(IThreadProvider))]
	public class ThreadProvider : IThreadProvider
	{
		public IList<IThreadMirror> Threads { get; private set; }
	}
}
