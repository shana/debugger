using CodeEditor.Composition;
using CodeEditor.IO.Implementation;
using CodeEditor.Text.UI.Unity.Engine;

namespace CodeEditor.Debugger.Unity.Engine
{
	[Export(typeof(ITextViewMarginProvider))]
	class BreakPointMarginProvider : ITextViewMarginProvider
	{
		[Import] private IDebugBreakPointProvider _debugBreakPointProvider;

		public ITextViewMargin MarginFor(ITextView textView)
		{
			return textView.Document.File is File ? CreateMargin(textView) : null;
		}

		private BreakPointMargin CreateMargin(ITextView textView)
		{
			return new BreakPointMargin(textView, _debugBreakPointProvider);
		}
	}
}
