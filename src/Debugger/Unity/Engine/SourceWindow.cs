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
				textView.Document.Caret.SetPosition (pendingSourceLine - 1, 0);
				currentDocument = System.IO.Path.GetFileName (pendingSourceLocation);
				textView.EnsureCursorIsVisible ();

				pendingSourceLocation = null;
			}

			if (textView == null)
				return;

			GUILayout.BeginArea (ViewPort, currentDocument, GUI.skin.window);
			textView.OnGUI ();
			GUILayout.EndArea ();
		}

		public void ShowSourceLocation (ILocation location)
		{
			pendingSourceLocation = location.SourceFile;
			pendingSourceLine = location.LineNumber;
		}

		public void RefreshSource ()
		{
			pendingSourceLine = textView.Document.CurrentLine.LineNumber;
		}
	}
}
