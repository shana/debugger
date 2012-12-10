using UnityEngine;

namespace Debugger.Unity.Engine
{
	public class DebuggerWindow : IDebuggerWindow
	{
		public static Rect Default = new Rect(0, 0, 0, 0);
		private Rect _rect;

		public DebuggerWindow()
		{
			_rect = Default;
		}

		public virtual void OnGUI ()
		{
			
		}

		public virtual string Title
		{
			get { return ""; }
		}

		public virtual Rect ViewPort
		{
			get { return _rect; }
			set { _rect = value; }
		}
	}
}
