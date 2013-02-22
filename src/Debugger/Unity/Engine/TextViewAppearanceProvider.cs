using CodeEditor.Composition;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export (typeof (ITextViewAppearanceProvider))]
	class TextViewAppearanceProvider : ITextViewAppearanceProvider
	{
		public ITextViewAppearance AppearanceFor (ITextViewDocument document)
		{
			return new TextViewAppearance ();
		}
	}

	public class TextViewAppearance : ITextViewAppearance
	{
		private readonly GUIStyle background;
		private readonly GUIStyle text;
		private readonly GUIStyle lineNumber;
		private readonly Color lineNumberColor;
		private readonly Color selectionColor;

		public TextViewAppearance ()
		{
			background = new GUIStyle ();

			text = new GUIStyle (background)
			{
				fontSize = 14,
				richText = true,
				alignment = TextAnchor.UpperLeft,
				padding = { left = 0, right = 0 }
			};
			text.normal.textColor = Color.white;

			lineNumber = new GUIStyle (text)
			{
				richText = false,
				alignment = TextAnchor.UpperRight
			};
			lineNumberColor = new Color (1, 1, 1, 0.5f);
		}

		public GUIStyle Background
		{
			get { return background; }
		}

		public GUIStyle Text
		{
			get { return text; }
		}

		public GUIStyle LineNumber
		{
			get { return lineNumber; }
		}

		public Color LineNumberColor
		{
			get { return lineNumberColor; }
		}

		public Color SelectionColor
		{
			get { return selectionColor; }
		}
	}
}
