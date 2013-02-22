using UnityEngine;

namespace Debugger.Unity.Engine
{
	public enum DockType
	{
		None,
		Top,
		TopLeft,
		TopRight,
		Bottom,
		BottomLeft,
		BottomRight
	}

	public abstract class DebuggerWindow : IDebuggerWindow
	{

		public static Rect Default = new Rect (0, 0, 0, 0);
		private Rect rect;

		public DebuggerWindow ()
		{
			rect = Default;
		}

		public virtual void OnGUI ()
		{

		}

		public virtual string Title
		{
			get { return ""; }
		}

		public virtual DockType DockType { get; set; }
		public virtual bool ExpandWidth { get; set; }
		public virtual bool ExpandHeight { get; set; }

		public virtual Rect ViewPort
		{
			get { return rect; }
			set { rect = value; }
		}

		public virtual void DrawHeader ()
		{
			GUILayout.BeginHorizontal ("Toolbar");

			GUILayout.Label (Title);

			GUILayout.FlexibleSpace ();

			GUILayout.EndHorizontal ();
		}
	}
}
