using System.Collections;
using System.Collections.Generic;
using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export]
	public class BreakpointsWindow : DebuggerWindow
	{
		private readonly IBreakpointProvider breakpointProvider;
		private readonly ITextView textView;
		private readonly ITextViewMargins margins;

		[ImportingConstructor]
		public BreakpointsWindow (ITextViewFactory viewFactory, IBreakpointProvider breakpointProvider)
		{
			this.breakpointProvider = breakpointProvider;
			this.breakpointProvider.BreakpointAdded += b => textView.Document.AppendLine (string.Format("{0}:{1}", b.Location.SourceFile,  b.Location.LineNumber));
			this.breakpointProvider.BreakpointRemoved += b => textView.Document.DeleteLine (this.breakpointProvider.IndexOf (b));
			textView = viewFactory.CreateView ();
			textView.ViewPort = DebuggerWindow.Default;
			margins = new MarginsFactory ().MarginsFor (this.textView);
		}

		public override Rect ViewPort
		{
			get { return textView.ViewPort; }
			set { textView.ViewPort = value; }
		}

		public override void OnGUI ()
		{
			textView.OnGUI ();
		}

		public override string Title
		{
			get { return "Breakpoints"; }
		}

		class MarginsFactory : ITextViewMarginsFactory
		{
			public ITextViewMargins MarginsFor (ITextView textView)
			{
				return new Margins () {new IconMargin ()};
			}

			class Margins : List<ITextViewMargin>, ITextViewMargins
			{
				public void Repaint (ITextViewLine line, Rect lineRect) {}
				public void HandleInputEvent (ITextViewLine line, Rect lineRect) {}
				public float TotalWidth { get; private set; }
			}

			class IconMargin : ITextViewMargin
			{
				[Import]
				public IBreakpointProvider BreakpointProvider { get; set; }

				GUIStyle styleOn = null;
				GUIStyle styleOff = null;

				public void Repaint (ITextViewLine line, Rect marginRect)
				{
					if (styleOn == null) {
						styleOn = MainWindow.skin.FindStyle ("eventpin");
						styleOff = MainWindow.skin.FindStyle ("eventpin on");
					}

					if (BreakpointProvider.IsBound (BreakpointProvider[line.LineNumber]))
						styleOn.Draw (marginRect, false, true, true, false);
					else
						styleOff.Draw (marginRect, false, true, true, false);
				}

				public void HandleInputEvent (ITextViewLine line, Rect marginRect)
				{
					if (Event.current.type != EventType.mouseDown)
						return;

					if (!marginRect.Contains (Event.current.mousePosition))
						return;

				}

				public float Width { get { return 10; } }
			}
		}
	}
}
