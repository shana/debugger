using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using Debugger.Backend;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export]
	[Export (typeof (IDebuggerWindow))]
	public class SourceWindow : DebuggerWindow
	{
		private readonly ITextViewFactory viewFactory;
		private ITextView textView;
		private volatile string pendingSourceLocation;
		private volatile int pendingSourceLine;
		private string currentDocument = "";
		private string title = "";

		public override string Title
		{
			get { return title; }
		}

		[ImportingConstructor]
		public SourceWindow (ITextViewFactory viewFactory)
		{
			this.viewFactory = viewFactory;
		}

		public override void OnGUI ()
		{
			if (pendingSourceLocation != null)
			{
				textView = viewFactory.ViewForFile (pendingSourceLocation);
				int topOffset = 25;
				int bottomOffset = 10;
				textView.ViewPort = new Rect (0, topOffset, ViewPort.width, ViewPort.height - topOffset - bottomOffset);
				textView.Document.Caret.SetPosition (pendingSourceLine, 0);
				currentDocument = pendingSourceLocation;
				title = System.IO.Path.GetFileName (currentDocument);
				textView.EnsureCursorIsVisible ();

				pendingSourceLocation = null;
				
			}

			if (textView == null)
				return;

			textView.OnGUI ();
		}

		public void ShowSourceLocation (ILocation location)
		{
			pendingSourceLocation = location.SourceFile;
			pendingSourceLine = location.LineNumber;
		}

		public void RefreshSource ()
		{
			pendingSourceLine = textView.Document.CurrentLine.LineNumber;
			pendingSourceLocation = currentDocument;
		}
	}
}
