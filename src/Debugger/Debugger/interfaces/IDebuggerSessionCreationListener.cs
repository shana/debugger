namespace CodeEditor.Debugger
{
	interface IDebuggerSessionCreationListener
	{
		void OnCreate(IDebuggerSession session);
	}
}
