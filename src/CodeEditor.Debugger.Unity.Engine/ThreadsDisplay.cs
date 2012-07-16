using System.Collections.Generic;
using CodeEditor.Composition;
using Mono.Debugger.Soft;
using UnityEngine;
using Event = Mono.Debugger.Soft.Event;

namespace CodeEditor.Debugger.Unity.Engine
{
	public class ThreadsDisplay : IDebuggerWindow
	{
		private readonly IDebuggerSession _debuggerSession;
		private IList<ThreadMirror> _threads = new List<ThreadMirror>();

		[ImportingConstructor]
		public ThreadsDisplay(IDebuggerSession debuggerSession)
		{
			_debuggerSession = debuggerSession;
			_debuggerSession.VMGotSuspended += VMGotSuspended;
		}

		private void VMGotSuspended(Event @event)
		{
			SetThreads(_debuggerSession.GetThreads());
		}

		public void OnGUI()
		{
			GUI.enabled = _debuggerSession.Suspended;

			foreach(var thread in _threads)
			{
				GUILayout.Label("Thread id: "+thread.Id+", Name: "+NameFor(thread));
			}
		}

		public string Title
		{
			get { return "Threads"; }
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