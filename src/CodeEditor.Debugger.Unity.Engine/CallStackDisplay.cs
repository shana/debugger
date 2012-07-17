using System;
using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using Mono.Debugger.Soft;
using UnityEngine;
using Event = Mono.Debugger.Soft.Event;

namespace CodeEditor.Debugger.Unity.Engine
{
	[Export(typeof(IDebuggerWindow))]
	public class CallStackDisplay : IDebuggerWindow
	{
		private readonly IDebuggerSession _debuggingSession;
		private readonly ISourceNavigator _sourceNavigator;
		private IEnumerable<StackFrame> _callFrames = new StackFrame[0];

		[ImportingConstructor]
		public CallStackDisplay(IDebuggerSession debuggingSession, ISourceNavigator sourceNavigator)
		{
			_debuggingSession = debuggingSession;
			_debuggingSession.VMGotSuspended += VMGotSuspended;
			_sourceNavigator = sourceNavigator;
		}

		private void VMGotSuspended(Event obj)
		{
			SetCallFrames(_debuggingSession.GetMainThread().GetFrames());
		}

		public void OnGUI()
		{
			GUI.enabled = _debuggingSession.Suspended;

			var backup = GUI.skin.button.alignment;
			GUI.skin.button.alignment = TextAnchor.MiddleLeft;
			foreach(var frame in _callFrames)
			{
				if (GUILayout.Button(frame.Method.DeclaringType.Name+"."+frame.Method.Name + " : " + frame.Location.LineNumber))
					_sourceNavigator.ShowSourceLocation(frame.Location);
			}
			if (!_callFrames.Any())
				GUILayout.Label("No stackframes on this threads stack");

			GUI.skin.button.alignment = backup;
		}

		public string Title
		{
			get { return "CallStack"; }
		}

		public void SetCallFrames(IEnumerable<StackFrame> frames)
		{
			_callFrames = frames;
		}
	}
}
