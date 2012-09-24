using System.IO;
using CodeEditor.Composition;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger.Unity.Engine
{
	public interface ISourceNavigator
	{
		void ShowSourceLocation(ILocation location);
	}

	[Export(typeof(ISourceNavigator))]
	internal class SourceNavigator : ISourceNavigator
	{
		[Import]
		public SourceWindow SourceWindow { get; set; }

		public void ShowSourceLocation(ILocation location)
		{
			if (!IsValidLocation(location))
				return;
			//Trace("{0}:{1}", location.SourceFile, location.LineNumber);
			SourceWindow.ShowSourceLocation(location.SourceFile, location.LineNumber);
		}

		private static bool IsValidLocation(ILocation location)
		{
			return location.LineNumber >= 1 && File.Exists(location.SourceFile);
		}
	}

}
