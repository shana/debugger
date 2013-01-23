using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	//[Export(typeof(ITextViewAdornment))]
	class ExecutingLineAdornment : ITextViewAdornment
	{
		IExecutionProvider ExecutionProvider { get; set; }

		public void Draw (ITextViewLine line, Rect lineRect)
		{
			if (line.LineNumber == ExecutionProvider.Location.LineNumber)
				Draw (lineRect);
		}

		private void Draw (Rect lineRect)
		{
			GUIUtils.DrawRect (lineRect, Color.red);
		}
	}
}
