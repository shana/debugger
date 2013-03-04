using System.Collections;
using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export]
	[Export (typeof (IDebuggerWindow))]
	[Export (typeof (ILogProvider))]
	public class LogWindow : DebuggerWindow, ILogProvider
	{
		private const int MaxLines = 200;
		private readonly ITextView textView;
		private readonly Queue pendingLines = Queue.Synchronized (new Queue ());

		[ImportingConstructor]
		public LogWindow (ITextViewFactory viewFactory)
		{
			textView = viewFactory.CreateView ();
			textView.ViewPort = DebuggerWindow.Default;
		}

		public override Rect ViewPort
		{
			get { return textView.ViewPort; }
			set { textView.ViewPort = value; }
		}

		public void WriteLine (string text)
		{
			pendingLines.Enqueue (text);
		}

		public override void OnGUI ()
		{
			FlushPendingLines ();
			textView.OnGUI ();
		}

		public override string Title
		{
			get { return "Log"; }
		}

		private void FlushPendingLines ()
		{
			var flushed = 0;
			while (pendingLines.Count > 0)
			{
				if (LineCount () > MaxLines) textView.Document.DeleteLine (0);
				textView.Document.AppendLine ((string)pendingLines.Dequeue ());
				if (++flushed > 10) break;
			}
		}

		private int LineCount ()
		{
			return textView.Document.LineCount;
		}

		public void Log (string msg)
		{
			textView.Document.AppendLine (msg);
		}
	}
}
