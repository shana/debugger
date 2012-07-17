using System.Collections.Generic;
using CodeEditor.Composition;
using Mono.Debugger.Soft;
using UnityEngine;
using Event = Mono.Debugger.Soft.Event;

namespace CodeEditor.Debugger.Unity.Engine
{
	[Export(typeof(IDebuggerWindow))]
	public class ThreadsDisplay : IDebuggerWindow
	{
		private readonly IDebuggerSession _debuggerSession;
		private IDebugThreadProvider _debugThreadProvider;
			
		[ImportingConstructor]
		public ThreadsDisplay(IDebuggerSession debuggerSession, IDebugThreadProvider debugThreadProvider)
		{
			_debuggerSession = debuggerSession;
			_debugThreadProvider = debugThreadProvider;
		}

		public void OnGUI()
		{
			GUI.enabled = _debuggerSession.Suspended;
			foreach(var thread in _debugThreadProvider.Threads)
			{
				GUILayout.Label("Thread id: "+thread.Id);
			}
		}

		public string Title
		{
			get { return "Threads"; }
		}
	}
}