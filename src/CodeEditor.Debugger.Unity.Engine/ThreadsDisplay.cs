using System.Collections.Generic;
using Mono.Debugger.Soft;
using UnityEngine;

namespace CodeEditor.Debugger.Unity.Engine
{
	public class ThreadsDisplay
	{
		private readonly DebuggerSession _debuggerSession;
		private IList<ThreadMirror> _threads = new List<ThreadMirror>();

		public ThreadsDisplay(DebuggerSession debuggerSession)
		{
			_debuggerSession = debuggerSession;
		}

		public void OnGUI()
		{
			if (!_debuggerSession.Suspended)
			{
				GUILayout.Label("No info (debugee is not suspended)");
				return;
			}

			foreach(var thread in _threads)
			{
				GUILayout.Label("Thread id: "+thread.Id+", Name: "+NameFor(thread));
			}
		}

		private static string NameFor(ThreadMirror thread)
		{
			return thread.IsThreadPoolThread ? "ThreadPool" : thread.Name;
		}

		public void SetThreads(IList<ThreadMirror> threads)
		{
			_threads = threads;
		}
	}
}