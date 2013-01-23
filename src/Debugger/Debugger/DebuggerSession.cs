using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debugger.Backend;
using Debugger.Backend.Event;
using Mono.Cecil;

namespace Debugger
{
	[CodeEditor.Composition.Export (typeof (IDebuggerSession))]
	public class DebuggerSession : IDebuggerSession
	{
		public IVirtualMachine VM { get; private set; }
		public ITypeProvider TypeProvider { get; private set; }
		public ISourceProvider SourceProvider { get; private set; }
		public IExecutionProvider ExecutionProvider { get; private set; }
		public IThreadProvider ThreadProvider { get; private set; }
		public IBreakpointProvider BreakpointProvider { get; set; }

		public event Action<string> TraceCallback;
		public event Action Loaded;

		static string[] Ignore = {
			"mscorlib",
			"System",
			"Unity",
			"ICSharpCode",
			"Mono",
			"I18N",
			"nunit",
			"Boo"
		};


		private ProcessStartInfo psi;
		private string options;
		private object obj = new object ();
		private IThreadMirror mainThread;
		private bool active;

		public string[] Params { get; set; }
		public int Port { get; set; }

		public bool Active
		{
			get
			{
				lock (obj)
				{
					return active;
				}
			}
		}

		public IThreadMirror MainThread
		{
			get
			{
				lock (obj)
				{
					return mainThread;
				}
			}
		}

		[CodeEditor.Composition.ImportingConstructor]
		public DebuggerSession (IVirtualMachine vm, ITypeProvider typeProvider,
			ISourceProvider sourceProvider, IExecutionProvider executionProvider,
			IThreadProvider threadProvider, IBreakpointProvider breakpointProvider)
		{
			this.TypeProvider = typeProvider;
			this.SourceProvider = sourceProvider;
			this.ExecutionProvider = executionProvider;
			this.ThreadProvider = threadProvider;
			this.BreakpointProvider = breakpointProvider;
			this.VM = vm;
		}

		//public DebuggerSession (ProcessStartInfo psi, string options)
		//    : this ()
		//{
		//    this.psi = psi;
		//    this.options = options;
		//    //var opts = new MDS.LaunchOptions () { AgentArgs = "loglevel=2,logfile=c:/as3/sdblog" };
		//}

		public void Start ()
		{
			LogProvider.Debug += LogOnDebug;
			LogProvider.Error += LogOnError;

			VM.OnVM += OnVm;
			VM.Attach (Port);
		}

		public void Stop ()
		{
//			lock (obj)
//			{
				active = false;
				mainThread = null;
//			}
			VM.OnVM -= OnVm;
			VM.Detach ();
		}

		private void OnVm (IEvent ev)
		{
			if (ev.State != State.Start) {
				LogOnDebug ("VM disconnected");
				return;
			}

			lock (obj)
			{
				if (active) {
				}
				active = true;
				mainThread = ev.Thread;
			}
		}

		private void LogOnError (string s)
		{
			if (TraceCallback != null)
				TraceCallback (s);
			else
				Console.WriteLine (s);
		}

		private void LogOnDebug (string s)
		{
			if (TraceCallback != null)
				TraceCallback (s);
		}


	}
}
