using System;
using System.Collections.Generic;
using System.IO;
using CodeEditor.Composition;
using CodeEditor.Debugger.Implementation;
using UnityEngine;

namespace CodeEditor.Debugger.Unity.Engine
{
	[Export]
	[Export (typeof (IDebuggerWindow))]
	public class SourcesWindow : DebuggerWindow
	{
		private ISourcesProvider _provider;
		private ISourceNavigator _sourceNavigator;

		[ImportingConstructor]
		public SourcesWindow(ISourcesProvider provider, ISourceNavigator sourceNavigator)
		{
			_provider = provider;
			_sourceNavigator = sourceNavigator;
		}

		public override void OnGUI()
		{
			GUI.enabled = true;
			var backup = GUI.skin.button.alignment;
			GUI.skin.button.alignment = TextAnchor.MiddleLeft;
			
			var files = new List<string> (_provider.Sources);
			foreach (var file in files)
			{
				if (GUILayout.Button(Path.GetFileName(file)))
					_sourceNavigator.ShowSourceLocation(new Location(1, file));
			}
		}


		public override string Title
		{
			get { return "Source files"; }
		}
	}
}
