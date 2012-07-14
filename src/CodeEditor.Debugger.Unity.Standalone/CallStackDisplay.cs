using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Debugger.Soft;
using UnityEngine;

namespace CodeEditor.Debugger.Unity.Standalone
{
	class CallStackDisplay
	{
		private readonly Action<StackFrame> _stackFrameSelectedCallback;
		private IEnumerable<StackFrame> _callFrames = new StackFrame[0];
		private Vector2 scrollPosition;

		public CallStackDisplay(Action<StackFrame> stackFrameSelectedCallback)
		{
			_stackFrameSelectedCallback = stackFrameSelectedCallback;
		}

		public void OnGUI()
		{
			int width = 500;
			GUILayout.BeginArea(new Rect(Screen.width-width,0,width,300));
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			GUILayout.BeginVertical();
			var backup = GUI.skin.button.alignment;
			GUI.skin.button.alignment = TextAnchor.MiddleLeft;
			foreach(var frame in _callFrames)
			{
				if (GUILayout.Button(frame.Method.FullName + " : " + frame.Location.LineNumber))
					_stackFrameSelectedCallback(frame);
			}
			GUI.skin.button.alignment = backup;
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		public void SetCallFrames(IEnumerable<StackFrame> frames)
		{
			_callFrames = frames;
		}
	}
}
