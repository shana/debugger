using System;
using System.Diagnostics;
using Debugger.Backend;
using Debugger.Backend.Sdb;
using MDS=Mono.Debugger.Soft;
namespace Debugger
{
	public class DebuggerSession : IDebuggerSession
	{
		public bool Active { get; private set; }

		public IVirtualMachine VM { get; private set; }

		public static IDebuggerSession Attach (int port)
		{
			return new DebuggerSession (port);
		}

		public static IDebuggerSession Launch (ProcessStartInfo psi, string options)
		{
			return new DebuggerSession (psi, options);
		}

		public event Action<string> TraceCallback;

		internal DebuggerSession (int port)
		{
			VM = VirtualMachineFactory.Connect (port);
		}

		internal DebuggerSession (ProcessStartInfo psi, string options)
		{
			var opts = new MDS.LaunchOptions () { AgentArgs = "loglevel=2,logfile=c:/as3/sdblog" };
		}
	}
}
