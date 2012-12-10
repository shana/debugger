using CodeEditor.Composition;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export(typeof(IDebuggerWindow))]
	public class ThreadsDisplay : DebuggerWindow
	{
		private readonly IDebuggerSession _debuggerSession;
		private IThreadProvider _threadProvider;

		[ImportingConstructor]
		public ThreadsDisplay(IDebuggerSession debuggerSession, IThreadProvider threadProvider)
		{
			_debuggerSession = debuggerSession;
			_threadProvider = threadProvider;
		}

		public override void OnGUI()
		{
			GUI.enabled = _debuggerSession.Active;
			foreach(var thread in _threadProvider.Threads)
			{
				GUILayout.Label("Thread id: "+thread.Id);
			}
		}

		public override string Title
		{
			get { return "Threads"; }
		}
	}
}
