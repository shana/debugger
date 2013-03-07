using CodeEditor.IO.Implementation;
using CodeEditor.Text.UI.Unity.Engine;
using Debugger.Backend;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	class BreakpointMargin : ITextViewMargin
	{
		private readonly ITextView textView;

		private readonly IBreakpointProvider breakpointProvider;

		public BreakpointMargin (ITextView textView, IBreakpointProvider breakpointProvider)
		{
			this.breakpointProvider = breakpointProvider;
			this.textView = textView;
		}

		public float Width
		{
			get { return 16; }
		}

		public void HandleInputEvent (ITextViewLine line, Rect marginRect)
		{
			if (Event.current.type != EventType.mouseDown)
				return;

			if (!marginRect.Contains (Event.current.mousePosition))
				return;

			SetBreakpoint (line);
		}

		public void Repaint (ITextViewLine line, Rect marginRect)
		{
			var breakpoint = GetBreakpoint (line);
			if (breakpoint != null)
				Draw (marginRect,
					breakpointProvider.IsBound (breakpoint), breakpoint.Enabled
					);
		}

		private void SetBreakpoint (ITextViewLine line)
		{
			breakpointProvider.ToggleBreakpointAt (((File)textView.Document.File).FullName, line.LineNumber + 1);
		}

		private IBreakpoint GetBreakpoint (ITextViewLine line)
		{
			return breakpointProvider.GetBreakpointAt (((File)textView.Document.File).FullName, line.LineNumber + 1);
		}

		private void Draw (Rect marginRect, bool on, bool enabled)
		{
			Texture texture = enabled ?  Styles.textureBreakpointEnabled : Styles.textureBreakpointDisabled;
			GUI.DrawTexture (marginRect, texture, ScaleMode.ScaleToFit);
			//style.Draw (marginRect, new GUIContent ("AA"), false, on, false, false);
		}
	}
}
