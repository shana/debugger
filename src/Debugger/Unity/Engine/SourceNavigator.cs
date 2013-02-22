using System.IO;
using CodeEditor.Composition;
using Debugger.Backend;

namespace Debugger.Unity.Engine
{
	public interface ISourceNavigator
	{
		void ShowSourceLocation (ILocation location);
		void RefreshSource ();
		ILocation CurrentLocation { get; }
	}

	[Export (typeof (ISourceNavigator))]
	internal class SourceNavigator : ISourceNavigator
	{
		[Import]
		public SourceWindow SourceWindow { get; set; }
		public ILocation CurrentLocation { get; private set; }

		public void RefreshSource ()
		{
			SourceWindow.RefreshSource ();
		}

		public void ShowSourceLocation (ILocation location)
		{
			if (!IsValidLocation (location))
				return;

			CurrentLocation = location;
			//Trace("{0}:{1}", location.SourceFile, location.LineNumber);
			SourceWindow.ShowSourceLocation (CurrentLocation);
		}

		static bool IsValidLocation (ILocation location)
		{
			LogProvider.Log ("checking {0} {1}", location.SourceFile, File.Exists (location.SourceFile));
			return location.LineNumber >= 1 && File.Exists (location.SourceFile);
		}
	}

}
