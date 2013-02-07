using System;
using Debugger.Backend;

namespace Debugger
{
	public interface IExecutionProvider
	{
		IThreadMirror CurrentThread { get; }
		ILocation Location { get; }
		bool Running { get; }

		event Action Break;
		event Action<IThreadMirror> Suspended;

		void Resume ();
		void Step ();
	}
}
