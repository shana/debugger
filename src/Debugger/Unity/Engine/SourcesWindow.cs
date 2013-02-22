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
		private ITypeProvider typeProvider;
		private ISourceNavigator sourceNavigator;

		[ImportingConstructor]
		public SourcesWindow (ITypeProvider typeProvider, ISourceNavigator sourceNavigator,
			ISourcesProvider sourcesProvider
)
		{
			this.typeProvider = typeProvider;
			this.sourceNavigator = sourceNavigator;
			this.sourcesProvider = sourcesProvider;
		}

		public void StartRefreshing ()
		{
			sourcesProvider.FileChanged += (f) => { if (sourceNavigator.CurrentLocation.SourceFile == f) sourceNavigator.RefreshSource (); };
			sourcesProvider.Start ();
		}

		IList<string> sourceFiles;
		private Vector2 scrollPosition;

		public override void OnGUI ()
		{
			if (Event.current.type == EventType.Layout)
				sourceFiles = sourcesProvider.SourceFiles;

			scrollPosition = GUILayout.BeginScrollView (scrollPosition, false, true);

			foreach (var file in sourceFiles)
			{
				if (sourceNavigator.CurrentLocation != null && sourceNavigator.CurrentLocation.SourceFile == file)
				{
					Color oldColor = GUI.contentColor;
					GUI.contentColor = new Color (0.42f, 0.7f, 1.0f, 1.0f);
					GUILayout.Label(Path.GetFileName (file));
					GUI.contentColor = oldColor;
					continue;
				}

				if (GUILayout.Button (Path.GetFileName (file), GUI.skin.label))
					sourceNavigator.ShowSourceLocation (new Location (file, 1));
			}

			GUILayout.EndScrollView ();
		}

		public override string Title
		{
			get { return "Source files"; }
		}
	}
}
