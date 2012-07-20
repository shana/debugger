using System;
using Mono.Debugger.Soft;

namespace CodeEditor.Debugger
{
	public interface IVirtualMachine
	{
		event Action<VMStartEvent> OnVMStart;
	}
}