using System.Collections;
using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace CodeEditor.Debugger.Unity.Engine
{
	[Export]
	public class LogWindow : IDebuggerWindow
	{
		private const int MaxLines = 200;
		private readonly ITextView _textView;
		private readonly Queue _pendingLines = Queue.Synchronized(new Queue());

		[ImportingConstructor]
		public LogWindow(ITextViewFactory viewFactory)
		{
			_textView = viewFactory.CreateView();
		}

		public Rect ViewPort
		{
			set { _textView.ViewPort = value; }
		}

		public void WriteLine(string text)
		{
			_pendingLines.Enqueue(text);
		}

		public void OnGUI()
		{
			FlushPendingLines();
			_textView.OnGUI();
		}

		public string Title
		{
			get { return "Log"; }
		}

		private void FlushPendingLines()
		{
			var flushed = 0;
			while (_pendingLines.Count > 0)
			{
				if (LineCount() > MaxLines) _textView.Document.DeleteLine(0);
				_textView.Document.AppendLine((string)_pendingLines.Dequeue());
				if (++flushed > 10) break;
			}
		}

		private int LineCount()
		{
			return _textView.Document.LineCount;
		}
	}
}
