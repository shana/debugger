using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace CodeEditor.Debugger.Unity.Engine
{
	[Export(typeof(ITextViewAdornment))]
	class ExecutingLineAdornment : ITextViewAdornment
	{
		[Import]
		ExecutingLineProvider ExecutingLineProvider { get; set; }

		public void Draw(ITextViewLine line, Rect lineRect)
		{
			if (line.LineNumber == ExecutingLineProvider.LineNumber)
				Draw(lineRect);
		}

		private void Draw(Rect lineRect)
		{
			GUIUtils.DrawRect(lineRect, Color.red);
		}
	}
}
