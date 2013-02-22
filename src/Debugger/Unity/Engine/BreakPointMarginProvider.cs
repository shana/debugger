using CodeEditor.Composition;
using CodeEditor.IO.Implementation;
using CodeEditor.Text.UI.Unity.Engine;

namespace Debugger.Unity.Engine
{
	[Export (typeof (ITextViewMarginProvider))]
	class BreakpointMarginProvider : ITextViewMarginProvider
	{
		[Import]
		public IBreakpointProvider BreakpointProvider { get; set; }

		public ITextViewMargin MarginFor (ITextView textView)
		{
			return textView.Document.File is File ? CreateMargin (textView) : null;
		}

		private BreakpointMargin CreateMargin (ITextView textView)
		{
			return new BreakpointMargin (textView, BreakpointProvider);
		}
	}
}
