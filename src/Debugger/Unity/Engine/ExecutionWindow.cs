using CodeEditor.Composition;
using Debugger.Unity.Engine;
using UnityEngine;

namespace Debugger.Unity.Standalone
{
	[Export]
	[Export (typeof (IDebuggerWindow))]
	public class ExecutionWindow : DebuggerWindow
	{
		private readonly IDebuggerSession session;
		private readonly IExecutionProvider executionProvider;

		[ImportingConstructor]
		public ExecutionWindow (IDebuggerSession session, IExecutionProvider executionProvider)
		{
			this.session = session;
			this.executionProvider = executionProvider;
		}

		public override void OnGUI()
		{
			GUI.enabled = session.Active && !executionProvider.Running;
			if (GUILayout.Button("Continue"))
				executionProvider.Resume ();
			if (GUILayout.Button("Step Over"))
				executionProvider.Step ();
			if (GUILayout.Button("Step In"))
				executionProvider.Step ();
			if (GUILayout.Button("Step Out"))
				executionProvider.Step ();

			GUI.enabled = session.Active && executionProvider.Running;
			if (GUILayout.Button("Break"))
			{
				
			}

			GUILayout.FlexibleSpace();
			GUI.enabled = true;


			if (session.Active) {
				if (GUILayout.Button("Stop"))
					session.Stop ();
			}
			else
			{
				if (GUILayout.Button("Start"))
					session.Start ();
			}


		}

		public string Title
		{
			get { return "Controls"; }
		}
	}
}
