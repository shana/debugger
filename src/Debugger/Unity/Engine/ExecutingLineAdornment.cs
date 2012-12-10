using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export(typeof(ITextViewAdornment))]
	class ExecutingLineAdornment : ITextViewAdornment
	{
		[Import]
		IExecutingLocationProvider ExecutingLocationProvider { get; set; }

		public void Draw(ITextViewLine line, Rect lineRect)
		{
			if (line.LineNumber == ExecutingLocationProvider.Location.LineNumber)
				Draw(lineRect);
		}

		private void Draw(Rect lineRect)
		{
			GUIUtils.DrawRect(lineRect, Color.red);
		}
	}
}
