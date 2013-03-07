using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debugger.Backend;
using Debugger.Backend.Event;
using Debugger.Backend.Sdb;

namespace Debugger
{
	[CodeEditor.Composition.Export (typeof (IDebuggerSession))]
	public class DebuggerSession : IDebuggerSession
	{
		private ProcessStartInfo psi;
		private string options;
		private object obj = new object ();
		private IThreadMirror mainThread;
		private bool active;
		private IVirtualMachine vm;
		private ITypeProvider typeProvider;
		private IExecutionProvider executionProvider;
		private IThreadProvider threadProvider;
		private IBreakpointProvider breakpointProvider;

		public IVirtualMachine VM {
			get {
				if (vm == null) {
					vm = new Debugger.Backend.Sdb.VirtualMachine ();
					//vm = Factory.CreateVirtualMachine ();
				}
				return vm;
			}
		}

		public ITypeProvider TypeProvider
		{
			get
			{
				if (typeProvider == null)
					typeProvider = new TypeProvider (VM);
				return typeProvider;
			}
		}

		public IExecutionProvider ExecutionProvider
		{
			get
			{
				if (executionProvider == null)
					executionProvider = new ExecutionProvider (VM);
				return executionProvider;
			}
		}

		public IThreadProvider ThreadProvider
		{
			get
			{
				if (threadProvider == null)
					threadProvider = new ThreadProvider (VM, ExecutionProvider);
				return threadProvider;
			}
		}

		public IBreakpointProvider BreakpointProvider
		{
			get
			{
				if (breakpointProvider == null)
					breakpointProvider = new BreakpointProvider (TypeProvider);
				return breakpointProvider;
			}
		}

		public event Action<string> TraceCallback;
		public event Action Loaded;

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

		public DebuggerSession ()
		{
		}

		[CodeEditor.Composition.ImportingConstructor]
		public DebuggerSession (IVirtualMachine vm, ITypeProvider typeProvider,
			IExecutionProvider executionProvider, IThreadProvider threadProvider,
			IBreakpointProvider breakpointProvider)
		{
			this.typeProvider = typeProvider;
			this.executionProvider = executionProvider;
			this.threadProvider = threadProvider;
			this.breakpointProvider = breakpointProvider;
			this.vm = vm;
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

			VM.VMStateChanged += OnVMStateChanged;
			VM.Attach (Port);
		}

		public void Stop ()
		{
//			lock (obj)
//			{
				active = false;
				mainThread = null;
//			}
			VM.VMStateChanged -= OnVMStateChanged;
			VM.Detach ();
		}

		private void OnVMStateChanged (IEvent ev)
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
