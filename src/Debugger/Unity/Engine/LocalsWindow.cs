using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CodeEditor.Composition;
using Debugger.Backend;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export]
	[Export (typeof (IDebuggerWindow))]
	public class LocalsWindow : DebuggerWindow
	{
		private readonly IDebuggerSession session;
		private readonly IExecutionProvider executionProvider;
		private IStackFrame pendingFrame, frame;
		private IList<IVariable> locals = new List<IVariable> ();
		private Vector2 scrollPosition;
		private bool running;

		[ImportingConstructor]
		public LocalsWindow (IDebuggerSession session, IExecutionProvider executionProvider)
		{
			this.session = session;
			this.executionProvider = executionProvider;
			this.executionProvider.Suspended += (thread) => {
					QueueUserWorkItem (() => pendingFrame = thread.GetFrames ().FirstOrDefault());
				};
		}

		private void QueueUserWorkItem (Action a)
		{
			ThreadPool.QueueUserWorkItem (_ => LogProvider.WithErrorLogging (a));
		}

		public override void OnGUI ()
		{
			if (Event.current.type == EventType.Layout)
			{
				if (frame != pendingFrame) {
					frame = pendingFrame;
					if (frame != null) {
						locals = frame.VisibleVariables;
						foreach (var local in locals)
							frame.GetValue (local);
					}
					else
						locals.Clear ();
					
					//QueueUserWorkItem (() => {
						//foreach(var v in locals.Select (x => frame.GetValue (x)))
						//	LogProvider.Log ("Variable:" + v);
					//});
				}
				running = executionProvider.Running;
			}

			scrollPosition = GUILayout.BeginScrollView (scrollPosition, false, true);

			GUI.enabled = session.Active && !running;

			if (!running)
			{
//				var backup = GUI.skin.button.alignment;
//				GUI.skin.button.alignment = TextAnchor.MiddleLeft;
				
				foreach (var local in locals)
				{
					GUILayout.BeginVertical ();
					GUILayout.BeginHorizontal ();
					GUILayout.Label (local.Name, GUILayout.Width (50));
					GUILayout.Label (local.ToString ());
					GUILayout.FlexibleSpace ();
					GUILayout.EndHorizontal ();
					GUILayout.EndVertical ();
				}
//				GUI.skin.button.alignment = backup;
			}
			GUI.enabled = true;
			GUILayout.EndScrollView ();
		}

		public override string Title
		{
			get { return "Locals"; }
		}
	}
}
