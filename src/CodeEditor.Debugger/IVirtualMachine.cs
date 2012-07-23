using System;

namespace CodeEditor.Debugger
{
	internal interface IVirtualMachine
	{
		event Action OnVMGotSuspended;
	}
}