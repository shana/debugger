using System;
using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Backend;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export]
	[Export (typeof (IDebuggerWindow))]
	public class BreakpointsWindow : DebuggerWindow
	{
		private readonly IDebuggerSession session;
		private readonly IBreakpointProvider breakpointProvider;
		private readonly ISourceNavigator sourceNavigator;
		private IList<IBreakpoint> breakpoints;
		private Vector2 scrollPosition;

		[ImportingConstructor]
		public BreakpointsWindow (IDebuggerSession session,
			IBreakpointProvider breakpointProvider,
			ISourceNavigator sourceNavigator
			)
		{
			this.session = session;
			this.breakpointProvider = breakpointProvider;
			this.sourceNavigator = sourceNavigator;
		}

		public override void OnGUI ()
		{
			if (Event.current.type == EventType.Layout)
				breakpoints = breakpointProvider.Breakpoints;

			scrollPosition = GUILayout.BeginScrollView (scrollPosition, false, true);

			GUI.enabled = session.Active;

			foreach (var bp in breakpoints)
			{
				var texture = bp.Enabled ? Styles.textureBreakpointEnabled : Styles.textureBreakpointDisabled;
				GUILayout.BeginHorizontal (GUILayout.ExpandWidth (true));
				var content = new GUIContent (string.Format("{0}:{1}", session.TypeProvider.MapRelativePath (bp.Location.SourceFile), bp.Location.LineNumber));
				GUI.DrawTexture (GUILayoutUtility.GetRect (20, 20), texture, ScaleMode.ScaleToFit);
				if (GUILayout.Button (content, Styles.skin.label))
					sourceNavigator.ShowSourceLocation (bp.Location);
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
			}

			GUI.enabled = true;
			GUILayout.EndScrollView ();
		}

		public override string Title
		{
			get { return "Breakpoints"; }
		}
	}
}
