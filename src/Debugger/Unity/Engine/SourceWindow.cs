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
		private string _currentDocument = "";

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
				int topOffset = 25;
				int bottomOffset = 10;
				_textView.ViewPort = new Rect(0, topOffset, ViewPort.width, ViewPort.height - topOffset - bottomOffset);
				_textView.Document.Caret.SetPosition(_pendingSourceLine-1,0);
				_currentDocument = System.IO.Path.GetFileName(_pendingSourceLocation);
				_textView.EnsureCursorIsVisible();

				_pendingSourceLocation = null;
			}

			if (_textView == null)
				return;

			GUILayout.BeginArea(ViewPort, _currentDocument, GUI.skin.window);
			_textView.OnGUI();
			GUILayout.EndArea();
		}

		public void ShowSourceLocation(string sourceFile, int lineNumber)
		{
			_pendingSourceLocation = sourceFile;
			_pendingSourceLine = lineNumber;
		}
	}
}
