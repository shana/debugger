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
	public class CallStackDisplay : DebuggerWindow
	{
		private readonly IDebuggerSession session;
		private readonly IExecutionProvider executionProvider;
		private readonly IThreadProvider threadProvider;
		private readonly ISourceNavigator sourceNavigator;
		private IEnumerable<IStackFrame> callFrames;
		private bool dirty;

		[ImportingConstructor]
		public CallStackDisplay (IDebuggerSession session, IExecutionProvider executionProvider,
			IThreadProvider threadProvider, ISourceNavigator sourceNavigator)
		{
			this.session = session;
			this.executionProvider = executionProvider;
			this.threadProvider = threadProvider;
			this.executionProvider.Suspended += RefreshFrames;
			this.sourceNavigator = sourceNavigator;
		}

		public override void OnGUI ()
		{
			GUI.enabled = session.Active && !executionProvider.Running;

			if (!executionProvider.Running)
			{
				var backup = GUI.skin.button.alignment;
				GUI.skin.button.alignment = TextAnchor.MiddleLeft;
				foreach (var frame in callFrames)
				{
					if (GUILayout.Button (frame.Method.DeclaringType.Name + "." + frame.Method.Name + " : " + frame.Location.LineNumber))
						sourceNavigator.ShowSourceLocation (frame.Location);
				}

				if (!callFrames.Any ())
					GUILayout.Label ("No stackframes on this threads stack");

				GUI.skin.button.alignment = backup;
			}
		}

		public override string Title
		{
			get { return "CallStack"; }
		}

		private void RefreshFrames (IThreadMirror thread)
		{
			callFrames = thread.GetFrames ();
		}
	}
}
