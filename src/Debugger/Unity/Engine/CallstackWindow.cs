using System;
using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Backend;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export]
	[Export (typeof (IDebuggerWindow))]
	public class CallstackWindow : DebuggerWindow
	{
		private readonly IDebuggerSession session;
		private readonly IExecutionProvider executionProvider;
		private readonly IThreadProvider threadProvider;
		private readonly ISourceNavigator sourceNavigator;
		private IList<IStackFrame> pendingFrames = new List<IStackFrame> ();
		private IList<IStackFrame> callFrames;
		private Vector2 scrollPosition;
		private bool running;

		[ImportingConstructor]
		public CallstackWindow (IDebuggerSession session, IExecutionProvider executionProvider,
			IThreadProvider threadProvider, ISourceNavigator sourceNavigator)
		{
			this.session = session;
			this.executionProvider = executionProvider;
			this.threadProvider = threadProvider;
			this.executionProvider.Suspended += (thread) => pendingFrames = thread.GetFrames ();;
			this.sourceNavigator = sourceNavigator;
		}

		public override void OnGUI ()
		{
			if (Event.current.type == EventType.Layout) {
				callFrames = pendingFrames.ToArray ();
				running = executionProvider.Running;
			}

			scrollPosition = GUILayout.BeginScrollView (scrollPosition, false, true);

			GUI.enabled = session.Active && !executionProvider.Running;

			if (!running)
			{
//				var backup = GUI.skin.button.alignment;
//				GUI.skin.button.alignment = TextAnchor.MiddleLeft;
				foreach (var frame in callFrames)
				{
					if (GUILayout.Button (frame.Method.DeclaringType.Name + "." + frame.Method.Name + " : " + frame.Location.LineNumber, Styles.skin.label))
						if (session.TypeProvider.TypesFor (frame.Location.SourceFile).Any())
							sourceNavigator.ShowSourceLocation (frame.Location);
				}

//				GUI.skin.button.alignment = backup;
			}

			GUI.enabled = true;
			GUILayout.EndScrollView ();
		}

		public override string Title
		{
			get { return "Callstack"; }
		}
	}
}
