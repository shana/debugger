using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Backend;
using Mono.Debugger.Soft;

namespace Debugger.Implementation
{
	[Export(typeof(IThreadProvider))]
	class ThreadProvider : IThreadProvider
	{
		readonly IDebuggerSession _session;

		private IList<DebugThread> _threads = new List<DebugThread>();

		[ImportingConstructor]
		public ThreadProvider(IDebuggerSession debuggingSession)
		{
			_session = debuggingSession;
			_session.VM.OnVMGotSuspended += VMGotSuspended;
		}

		private void VMGotSuspended(IEvent obj)
		{
			var threads = _session.VM.GetThreads ();
			_threads = threads.Select(t => new DebugThread(t.Id)).ToList();
		}

		public IList<DebugThread> Threads {
			get { return _threads; }
		}
	}

	public class DebugThread
	{
		public DebugThread(long id)
		{
			Id = id;
		}

		public long Id { get; private set; }
	}
}
