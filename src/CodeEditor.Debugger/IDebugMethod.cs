using System.Collections.Generic;

namespace CodeEditor.Debugger
{
	public interface IDebugMethod
	{
		IEnumerable<IDebugLocation> Locations { get; }
	}
}