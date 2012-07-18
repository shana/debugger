using CodeEditor.Composition;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Implementation
{
	[Export]
	public class ExecutingLineProvider
	{
		private int _currentLocation;
		private readonly IDebuggerSession _debuggerSession;

		[ImportingConstructor]
		public ExecutingLineProvider(IDebuggerSession debuggerSession)
		{
			_debuggerSession = debuggerSession;
			_debuggerSession.VMGotSuspended += VMGotSuspended;
		}

		private void VMGotSuspended(Event e)
		{
			var frames = e.Thread.GetFrames();
			_currentLocation = frames.Length==0 ? 0 : frames[0].LineNumber - 1;
		}

		public int LineNumber
		{
			get { return _currentLocation; }
		}
	}
}