using CodeEditor.Composition;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export]
	[Export (typeof (IDebuggerWindow))]
	public class ThreadsDisplay : DebuggerWindow
	{
		private readonly IDebuggerSession session;
		private IThreadProvider threadProvider;

		[ImportingConstructor]
		public ThreadsDisplay (IDebuggerSession session, IThreadProvider threadProvider)
		{
			this.session = session;
			this.threadProvider = threadProvider;
		}

		public override void OnGUI ()
		{
			GUI.enabled = session.Active;
			if (session.Active)
				foreach (var thread in threadProvider.Threads)
					GUILayout.Label ("Thread id: " + thread.Id);
		}

		public override string Title
		{
			get { return "Threads"; }
		}
	}
}
