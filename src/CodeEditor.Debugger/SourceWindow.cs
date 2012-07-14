using CodeEditor.Composition;
using CodeEditor.Text.UI.UnityEngine;
using UnityEngine;

namespace CodeEditor.Debugger
{
	[Export]
	class SourceWindow
	{
		private readonly ITextViewFactory _viewFactory;
		private ITextView _textView;
		private volatile string _pendingSourceLocation;

		[ImportingConstructor]
		public SourceWindow(ITextViewFactory viewFactory)
		{
			_viewFactory = viewFactory;
		}

		public Rect ViewPort { get; set; }

		public void OnGUI()
		{
			if (_pendingSourceLocation != null)
			{
				_textView = _viewFactory.ViewForFile(_pendingSourceLocation);
				_textView.ViewPort = ViewPort;
				_pendingSourceLocation = null;
			}

			if (_textView == null)
				return;

			_textView.OnGUI();
		}

		public void ShowSourceLocation(string sourceFile, int lineNumber)
		{
			_pendingSourceLocation = sourceFile;
		}
	}
}
