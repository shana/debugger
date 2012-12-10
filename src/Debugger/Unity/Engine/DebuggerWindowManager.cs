using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	public interface IDebuggerWindow
	{
		void OnGUI();
		string Title { get; }
		Rect ViewPort { get; }
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
			_windows = windows._importedWindows.Where (w => w.ViewPort.Equals (DebuggerWindow.Default)).ToList ();
			_customWindows = windows._importedWindows.Where (w => !w.ViewPort.Equals (DebuggerWindow.Default)).ToList ();
		}

		private readonly List<IDebuggerWindow> _windows;
		private readonly List<IDebuggerWindow> _customWindows;

		public Rect ViewPort { get; set; }

		public T Get<T>() where T:class
		{
			return (_windows.FirstOrDefault(w => w is T) ?? _customWindows.FirstOrDefault(w => w is T)) as T;
		}

		public void Add(IDebuggerWindow debuggerWindow)
		{
			if (debuggerWindow.ViewPort.Equals(DebuggerWindow.Default))
				_windows.Add(debuggerWindow);
			else
				_customWindows.Add (debuggerWindow);
		}

		public void ResetWindows ()
		{
			_windows.AddRange(_customWindows);
			_customWindows.Clear();
			_customWindows.AddRange(_windows.Where (w => !w.ViewPort.Equals (DebuggerWindow.Default)));
			_windows.RemoveAll(w => !w.ViewPort.Equals(DebuggerWindow.Default));
		}

		public void OnGUI()
		{
			foreach (var window in _customWindows)
			{
				GUI.enabled = true;
				GUILayout.BeginArea (window.ViewPort, window.Title, GUI.skin.window);
				window.OnGUI ();
				GUILayout.EndArea ();
			}

			GUI.enabled = true;
			GUILayout.BeginArea (ViewPort);

			int windowCount = _windows.Count();
			int gaps = windowCount - 1;
			int gapwidth = 10;
			int width = (Screen.width - gaps*gapwidth)/windowCount;

			var rect = new Rect(0,0, width,ViewPort.height);

			foreach(var window in _windows)
			{
				GUI.enabled = true;
				GUILayout.BeginArea (rect, window.Title, GUI.skin.window);
				window.OnGUI ();
				GUILayout.EndArea();
				rect.x = rect.x + width + gapwidth;
			}
				
			GUILayout.EndArea();
		}
	}
}
