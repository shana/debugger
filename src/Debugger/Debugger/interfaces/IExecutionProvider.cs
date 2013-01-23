using System;
using Debugger.Backend;

namespace Debugger
{
	public interface IExecutionProvider
	{
		bool Running { get; }
		ILocation Location { get; }
		IThreadMirror CurrentThread { get; }
		void Resume ();
		event Action<IThreadMirror> Suspended;
		event Action Break;
		void Step ();
	}
}
