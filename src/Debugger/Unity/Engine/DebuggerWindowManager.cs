using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	public interface IDebuggerWindow
	{
		void OnGUI ();
		string Title { get; }
		Rect ViewPort { get; set; }
		DockType DockType { get; set; }
		bool ExpandWidth { get; set; }
		bool ExpandHeight { get; set; }
		void DrawHeader ();
	}

	[Export]
	public class DebuggerWindowManager
	{
		private readonly List<IDebuggerWindow> windows = new List<IDebuggerWindow> ();
		private readonly List<IDebuggerWindow> customWindows = new List<IDebuggerWindow> ();

		public Rect ViewPort { get; set; }

		//[Export]
		//public class ImportedWindows
		//{
		//    [ImportMany]
		//    internal IDebuggerWindow[] importedWindows;
		//}

		//[ImportingConstructor]
		//public DebuggerWindowManager (ImportedWindows windows)
		//{
		//    this.windows = windows.importedWindows.Where (w => w.ViewPort.Equals (DebuggerWindow.Default)).ToList ();
		//    customWindows = windows.importedWindows.Where (w => !w.ViewPort.Equals (DebuggerWindow.Default)).ToList ();
		//}

		public T Get<T> () where T : class
		{
			return (windows.FirstOrDefault (w => w is T) ?? customWindows.FirstOrDefault (w => w is T)) as T;
		}

		public void Add (IDebuggerWindow debuggerWindow)
		{
			if (debuggerWindow.ViewPort.Equals (DebuggerWindow.Default))
				windows.Add (debuggerWindow);
			else
				customWindows.Add (debuggerWindow);
		}

		public void ResetWindows ()
		{
			windows.AddRange (customWindows);
			customWindows.Clear ();
			customWindows.AddRange (windows.Where (w => !w.ViewPort.Equals (DebuggerWindow.Default)));
			windows.RemoveAll (w => !w.ViewPort.Equals (DebuggerWindow.Default));
		}

		public void OnGUI ()
		{
			int padding = 2;
			float currentX = 0;

			foreach (var window in customWindows)
			{
				var x = currentX;
				var y = window.ViewPort.y;
				var w = window.ViewPort.width;
				var h = window.ViewPort.height;
				if (window.ExpandWidth)
					w = Screen.width - currentX;

				window.ViewPort = new Rect(x, y, w, h);

				GUI.enabled = true;
				GUILayout.BeginArea (window.ViewPort, "", "CN Box");
				window.DrawHeader ();
				window.OnGUI ();
				GUILayout.EndArea ();

				currentX += w + padding;
			}

			GUI.enabled = true;
			GUILayout.BeginArea (ViewPort);

			int windowCount = windows.Count ();
			int gaps = windowCount - 1;
			int width = (Screen.width - gaps * padding) / windowCount;

			var rect = new Rect (0, 0, width, ViewPort.height);

			foreach (var window in windows)
			{
				GUI.enabled = true;
				GUILayout.BeginArea (rect, "", "CN Box");
				window.DrawHeader ();
				window.OnGUI ();
				GUILayout.EndArea ();
				rect.x = rect.x + width + padding;
			}

			GUILayout.EndArea ();
		}
	}
}
