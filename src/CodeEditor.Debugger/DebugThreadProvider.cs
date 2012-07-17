using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeEditor.Composition;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger
{
	public interface IDebugThreadProvider
	{
		IList<DebugThread> Threads { get; }
	}

	[Export(typeof(IDebugThreadProvider))]
	public class DebugThreadProvider : IDebugThreadProvider
	{
		readonly IDebuggerSession _debuggingSession;

		private IList<DebugThread> _threads = new List<DebugThread>();

		[ImportingConstructor]
		public DebugThreadProvider(IDebuggerSession debuggingSession)
		{
			_debuggingSession = debuggingSession;
			_debuggingSession.VMGotSuspended += VMGotSuspended;
		}

		private void VMGotSuspended(Event obj)
		{
			var threads = _debuggingSession.GetThreads();
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
