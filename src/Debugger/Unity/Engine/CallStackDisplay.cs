using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Backend;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export(typeof(IDebuggerWindow))]
	public class CallStackDisplay : DebuggerWindow
	{
		private readonly IDebuggerSession _debuggingSession;
		private readonly ISourceNavigator _sourceNavigator;
		private IEnumerable<IStackFrame> _callFrames;

		[ImportingConstructor]
		public CallStackDisplay(IDebuggerSession debuggingSession, ISourceNavigator sourceNavigator)
		{
			_debuggingSession = debuggingSession;
			_sourceNavigator = sourceNavigator;
			_debuggingSession.VM.OnVMGotSuspended += VMGotSuspended;
		}

		private void VMGotSuspended (IEvent obj)
		{
			var thread = _debuggingSession.VM.GetThreads().FirstOrDefault();
			if (thread != null)
				SetCallFrames(thread.GetFrames());
		}

		public override void OnGUI ()
		{
			GUI.enabled = _debuggingSession.Active;

			var backup = GUI.skin.button.alignment;
			GUI.skin.button.alignment = TextAnchor.MiddleLeft;

			//foreach(var frame in _callFrames)
			//{
			//    if (GUILayout.Button(frame.Method.DeclaringType.Name+"."+frame.Method.Name + " : " + frame.Location.LineNumber))
			//        _sourceNavigator.ShowSourceLocation(Location.FromLocation(frame.Location));
			//}
			//if (!_callFrames.Any())
			//    GUILayout.Label("No stackframes on this threads stack");

			GUI.skin.button.alignment = backup;
		}

		public override string Title
		{
			get { return "CallStack"; }
		}

		public void SetCallFrames (IEnumerable<IStackFrame> frames)
		{
			_callFrames = frames;
		}
	}
}
