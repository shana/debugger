using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	static class Styles
	{
//		public static GUIStyle breakpointDisabled;
//		public static GUIStyle breakpointEnabled;
		public static GUISkin skin;
		public static Texture textureBreakpointDisabled;
		public static Texture textureBreakpointEnabled;

		static Styles ()
		{
			skin = Resources.Load("Skins/Generated/DarkSkin", typeof(GUISkin)) as GUISkin;
			Debug.Log ("skin " + skin);
//			breakpointDisabled = "flow node hex 0";
//			breakpointEnabled = "flow node hex 6";
			textureBreakpointDisabled = Resources.Load ("Skins/DarkSkin/FlowImages/node6 hex", typeof(Texture)) as Texture;
			textureBreakpointEnabled = Resources.Load ("Skins/DarkSkin/FlowImages/node6 hex on", typeof(Texture)) as Texture;
		}
	}
}
