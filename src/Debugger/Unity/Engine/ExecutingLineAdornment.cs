using System;
using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using Debugger.Backend;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export(typeof(ITextViewAdornment))]
	public class ExecutingLineAdornment : ITextViewAdornment
	{
		private readonly IExecutionProvider executionProvider;

		[ImportingConstructor]
		public ExecutingLineAdornment (IExecutionProvider executionProvider)
		{
			this.executionProvider = executionProvider;
		}

		public void Draw (ITextViewLine line, Rect lineRect)
		{
			if (executionProvider.Running)
				return;
			if (line.LineNumber == executionProvider.Location.LineNumber - 1)
				Draw (lineRect);
		}

		private void Draw (Rect lineRect)
		{
			GUIUtils.DrawRect (lineRect, Color.red);
		}
	}
}
