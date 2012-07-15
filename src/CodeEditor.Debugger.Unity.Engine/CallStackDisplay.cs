using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Debugger.Soft;
using UnityEngine;

namespace CodeEditor.Debugger.Unity.Engine
{
	public class CallStackDisplay
	{
		private readonly Action<StackFrame> _stackFrameSelectedCallback;
		private IEnumerable<StackFrame> _callFrames = new StackFrame[0];
		
		public CallStackDisplay(Action<StackFrame> stackFrameSelectedCallback)
		{
			_stackFrameSelectedCallback = stackFrameSelectedCallback;
		}

		public void OnGUI()
		{
			var backup = GUI.skin.button.alignment;
			GUI.skin.button.alignment = TextAnchor.MiddleLeft;
			foreach(var frame in _callFrames)
			{
				if (GUILayout.Button(frame.Method.FullName + " : " + frame.Location.LineNumber))
					_stackFrameSelectedCallback(frame);
			}
			if (!_callFrames.Any())
				GUILayout.Label("No stackframes on this threads stack");

			GUI.skin.button.alignment = backup;
		}

		public void SetCallFrames(IEnumerable<StackFrame> frames)
		{
			_callFrames = frames;
		}
	}
}
