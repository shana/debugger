using System.IO;
using CodeEditor.Composition;
using Debugger.Backend;

namespace Debugger.Unity.Engine
{
	public interface ISourceNavigator
	{
		void ShowSourceLocation (ILocation location);
		void RefreshSource ();
		ILocation CurrentSource { get; }
	}

	[Export (typeof (ISourceNavigator))]
	internal class SourceNavigator : ISourceNavigator
	{
		[Import]
		public SourceWindow SourceWindow { get; set; }
		public ILocation CurrentSource { get; private set; }

		public void RefreshSource ()
		{
			SourceWindow.RefreshSource ();
		}

		public void ShowSourceLocation (ILocation location)
		{
			if (!IsValidLocation (location))
				return;

			CurrentSource = location;
			//Trace("{0}:{1}", location.SourceFile, location.LineNumber);
			SourceWindow.ShowSourceLocation (CurrentSource);
		}

		static bool IsValidLocation (ILocation location)
		{
			return location.LineNumber >= 1 && File.Exists (location.SourceFile);
		}
	}

}
