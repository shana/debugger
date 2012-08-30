using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using UnityEngine;

namespace CodeEditor.Debugger.Unity.Engine
{
	public interface IDebuggerWindow
	{
		void OnGUI();
		string Title { get; }
	}

	[Export]
	public class DebuggerWindowManager
	{
		[Export]
		public class ImportedWindows
		{
			[ImportMany]
			internal IDebuggerWindow[] _importedWindows;
		}

		[ImportingConstructor]
		public DebuggerWindowManager(ImportedWindows windows)
		{
			_windows = windows._importedWindows.ToList();
		}

		private readonly List<IDebuggerWindow> _windows;

		public Rect ViewPort { get; set; }

		public void Add(IDebuggerWindow debuggerWindow)
		{
			_windows.Add(debuggerWindow);
		}

		public void OnGUI()
		{
			GUILayout.BeginArea(ViewPort);

			int windowCount = _windows.Count();
			int gaps = windowCount - 1;
			int gapwidth = 10;
			int width = (Screen.width - gaps*gapwidth)/windowCount;

			var rect = new Rect(0,0, width,ViewPort.height);

			foreach(var window in _windows)
			{
				GUI.enabled = true;
				GUILayout.BeginArea(rect, window.Title, GUI.skin.window);
				window.OnGUI();
				GUILayout.EndArea();
				rect.x = rect.x + width + gapwidth;
			}
			GUILayout.EndArea();
		}
	}
}
