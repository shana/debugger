using System.Collections;
using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export]
	[Export(typeof(IDebuggerWindow))]
	[Export(typeof(ILogProvider))]
	public class LogWindow : DebuggerWindow, ILogProvider
	{
		private const int MaxLines = 200;
		private readonly ITextView _textView;
		private readonly Queue _pendingLines = Queue.Synchronized(new Queue());

		[ImportingConstructor]
		public LogWindow(ITextViewFactory viewFactory)
		{
			_textView = viewFactory.CreateView();
		}

		public override Rect ViewPort
		{
			get { return _textView.ViewPort; }
			set { _textView.ViewPort = value; }
		}

		public void WriteLine(string text)
		{
			_pendingLines.Enqueue(text);
		}

		public override void OnGUI ()
		{
			FlushPendingLines();
			_textView.OnGUI();
		}

		public override string Title
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

		public void Log(string msg)
		{
			_textView.Document.AppendLine(msg);
		}
	}
}
