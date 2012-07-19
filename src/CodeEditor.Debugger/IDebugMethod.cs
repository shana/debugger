using System.Collections.Generic;

namespace CodeEditor.Debugger
{
	public interface IDebugMethod
	{
		IDebugLocation[] Locations { get; }
	}
}