using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MDS=Mono.Debugger.Soft;

namespace Debugger.Backend.Sdb
{
	public class VirtualMachineFactory
	{
		public static IVirtualMachine Connect (int debuggerPort)
		{
			Console.WriteLine ("Attempting connection at port {0}...", debuggerPort);
			var vm = MDS.VirtualMachineManager.Connect (new IPEndPoint(IPAddress.Loopback, debuggerPort));
			return new VirtualMachine (vm);
		}
	}
}
