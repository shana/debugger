using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Backend;

namespace Debugger
{
	[Export (typeof (IThreadProvider))]
	public class ThreadProvider : IThreadProvider
	{
		private readonly IVirtualMachine vm;
		private readonly IExecutionProvider executionProvider;
		private IList<IThreadMirror> threads = null;

		[ImportingConstructor]
		public ThreadProvider (IVirtualMachine vm, IExecutionProvider executionProvider)
		{
			this.vm = vm;
			this.executionProvider = executionProvider;
			executionProvider.Suspended += Suspended;
		}

		private void Suspended (IThreadMirror thread)
		{
			threads = null;
			CurrentThread = thread;
		}

		public IEnumerable<IThreadMirror> Threads
		{
			get
			{
				if (threads == null)
					this.threads = vm.GetThreads ().ToList ();
				return threads;
			}
		}

		public IThreadMirror CurrentThread { get; private set; }
	}
}
