using CodeEditor.IO.Implementation;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace CodeEditor.Debugger.Unity.Engine
{
	class BreakPointMargin : ITextViewMargin
	{
		private readonly ITextView _textView;

		private readonly IBreakpointProvider _breakpointProvider;

		public BreakPointMargin(ITextView textView, IBreakpointProvider breakpointProvider)
		{
			_breakpointProvider = breakpointProvider;
			_textView = textView;
		}

		public float Width
		{
			get { return 16; }
		}

		public void HandleInputEvent(ITextViewLine line, Rect marginRect)
		{
			if (Event.current.type != EventType.mouseDown)
				return;

			if (!marginRect.Contains(Event.current.mousePosition))
				return;

			SetBreakPoint(line);
		}

		public void Repaint(ITextViewLine line, Rect marginRect)
		{
			if (GetBreakPoint(line) != null)
				Draw(marginRect);
		}

		private void SetBreakPoint(ITextViewLine line)
		{
			_breakpointProvider.ToggleBreakPointAt(File().FullName, line.LineNumber);
		}

		private IBreakPoint GetBreakPoint(ITextViewLine line)
		{
			return _breakpointProvider.GetBreakPointAt(File().FullName, line.LineNumber);
		}

		private File File()
		{
			return (File) _textView.Document.File;
		}

		private void Draw(Rect marginRect)
		{
			GUIUtils.DrawRect(marginRect,Color.red);
		}
	}
}
