using System.Collections;
using System.Collections.Generic;
using System.IO;
using CodeEditor.Composition;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export]
	[Export (typeof (IDebuggerWindow))]
	public class SourcesWindow : DebuggerWindow
	{
		private ISourcesProvider sourcesProvider;
		private ISourceNavigator sourceNavigator;

		[ImportingConstructor]
		public SourcesWindow (ISourcesProvider sourcesProvider, ISourceNavigator sourceNavigator)
		{
			this.sourcesProvider = sourcesProvider;
			this.sourceNavigator = sourceNavigator;
		}

		public void StartRefreshing ()
		{
			sourcesProvider.FileChanged += (f) => { if (sourceNavigator.CurrentSource.SourceFile == f) sourceNavigator.RefreshSource (); };
			sourcesProvider.StartRefreshingSources ();
		}

		public override void OnGUI ()
		{
			GUI.enabled = true;
			var backup = GUI.skin.button.alignment;
			GUI.skin.button.alignment = TextAnchor.MiddleLeft;

			foreach (var file in sourcesProvider.Sources)
			{
				if (GUILayout.Button (Path.GetFileName (file)))
					sourceNavigator.ShowSourceLocation (new Location (file, 1));
			}
		}

		public override string Title
		{
			get { return "Source files"; }
		}
	}
}
