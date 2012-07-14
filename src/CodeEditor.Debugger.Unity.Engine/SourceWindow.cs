using System;
using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace CodeEditor.Debugger.Unity.Engine
{
	[Export]
	public class SourceWindow
	{
		private readonly ITextViewFactory _viewFactory;
		private ITextView _textView;
		private volatile string _pendingSourceLocation;
		private volatile int _pendingSourceLine;

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
				_textView.Document.Caret.SetPosition(_pendingSourceLine-1,0);

				_textView.EnsureCursorIsVisible();

				_pendingSourceLocation = null;
			}

			if (_textView == null)
				return;

			_textView.OnGUI();
		}

		public void ShowSourceLocation(string sourceFile, int lineNumber)
		{
			_pendingSourceLocation = sourceFile;
			_pendingSourceLine = lineNumber;
		}
	}
}
