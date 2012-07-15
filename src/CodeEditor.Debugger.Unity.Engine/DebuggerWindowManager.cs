using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CodeEditor.Debugger.Unity.Engine
{
	public class DebuggerWindowManager
	{
		struct Window
		{
			public string title;
			public Action onGUI;
		}

		private readonly List<Window> _windows = new List<Window>();
		private Vector2 scrollPosition;

		public Rect ViewPort { get; set; }

		public void Add(string title, Action onGUI)
		{
			_windows.Add(new Window() { onGUI = onGUI, title = title});
		}

		public void OnGUI()
		{
			GUILayout.BeginArea(ViewPort);
			var backup = GUI.backgroundColor;
			GUI.backgroundColor = new Color(.4f, .4f, .4f, 1);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			GUILayout.BeginVertical();
			foreach(var window in _windows)
			{
				GUILayout.Label(window.title);
				window.onGUI();
			}
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			GUI.backgroundColor = backup;
		}
	}
}
