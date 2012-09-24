using System;
using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using CodeEditor.Debugger.Implementation;
using UnityEngine;
using mds = Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Unity.Engine
{
	[Export(typeof(IDebuggerWindow))]
	public class CallStackDisplay : DebuggerWindow
	{
		private readonly IDebuggerSession _debuggingSession;
		private readonly ISourceNavigator _sourceNavigator;
		private IEnumerable<mds.StackFrame> _callFrames = new mds.StackFrame[0];

		[ImportingConstructor]
		public CallStackDisplay(IDebuggerSession debuggingSession, ISourceNavigator sourceNavigator)
		{
			_debuggingSession = debuggingSession;
			_debuggingSession.VMGotSuspended += VMGotSuspended;
			_sourceNavigator = sourceNavigator;
		}

		private void VMGotSuspended (mds.Event obj)
		{
			SetCallFrames(_debuggingSession.GetMainThread().GetFrames());
		}

		public override void OnGUI ()
		{
			GUI.enabled = _debuggingSession.Suspended;

			var backup = GUI.skin.button.alignment;
			GUI.skin.button.alignment = TextAnchor.MiddleLeft;
			foreach(var frame in _callFrames)
			{
				if (GUILayout.Button(frame.Method.DeclaringType.Name+"."+frame.Method.Name + " : " + frame.Location.LineNumber))
					_sourceNavigator.ShowSourceLocation(Location.FromLocation(frame.Location));
			}
			if (!_callFrames.Any())
				GUILayout.Label("No stackframes on this threads stack");

			GUI.skin.button.alignment = backup;
		}

		public override string Title
		{
			get { return "CallStack"; }
		}

		public void SetCallFrames (IEnumerable<mds.StackFrame> frames)
		{
			_callFrames = frames;
		}
	}
}
