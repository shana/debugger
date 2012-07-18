using CodeEditor.Composition;

namespace CodeEditor.Debugger
{
	public interface IDebuggerSessionAssembler
	{
		void Assemble();
	}

	[Export(typeof(IDebuggerSessionAssembler))]
	public class DebuggerSessionAssembler : IDebuggerSessionAssembler
	{
		[Import] private IDebuggerSession _session;
		[ImportMany]  private IDebuggerSessionCreationListener[] _listeners;

		public void Assemble()
		{
			foreach(var listener in _listeners)
				listener.OnCreate(_session);
		}
	}
}
