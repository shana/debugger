using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeEditor.Composition;
using CodeEditor.IO.Implementation;
using CodeEditor.Text.UI.Unity.Engine;
using UnityEngine;

namespace CodeEditor.Debugger.Unity.Engine
{
	class BreakPointMargin : ITextViewMargin
	{
		private readonly ITextView _textView;
		private Texture2D _texture;

		private readonly IDebugBreakPointProvider _debugBreakPointProvider;

		public BreakPointMargin(ITextView textView, IDebugBreakPointProvider debugBreakPointProvider)
		{
			_debugBreakPointProvider = debugBreakPointProvider;
			_textView = textView;
			CreateTexture();
		}

		public float Width
		{
			get { return _texture.width; }
		}

		public void DoGUI(ITextViewLine line, Rect marginRect)
		{
			switch (Event.current.type)
			{
				case EventType.mouseDown:
					HandleMouseDown(line, marginRect);
					return;
				case EventType.repaint:
					HandleRepaint(line, marginRect);
					return;
			}
		}

		private void HandleRepaint(ITextViewLine line, Rect marginRect)
		{
			if (GetBreakPoint(line) != null)
				Draw(marginRect);
		}

		private void HandleMouseDown(ITextViewLine line, Rect marginRect)
		{
			if (!marginRect.Contains(Event.current.mousePosition))
				return;

			SetBreakPoint(line);
		}

		private void SetBreakPoint(ITextViewLine line)
		{
			_debugBreakPointProvider.ToggleBreakPointAt(File().FullName, line.LineNumber);
		}

		private IBreakPoint GetBreakPoint(ITextViewLine line)
		{
			return _debugBreakPointProvider.GetBreakPointAt(File().FullName, line.LineNumber);
		}

		private File File()
		{
			return (File) _textView.Document.File;
		}

		private void Draw(Rect marginRect)
		{
			GUI.DrawTexture(marginRect,_texture,ScaleMode.StretchToFill, false);
		}

		private void CreateTexture()
		{
			_texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
			for (int x = 0; x != _texture.width; x++)
				for (int y = 0; y != _texture.height; y++)
					_texture.SetPixel(x, y, new Color(1f, 1f, 0, .5f));
		}
	}
}
