using CodeEditor.Composition;
using Debugger.Backend;
using UnityEngine;

namespace Debugger.Unity.Engine
{
	[Export]
	[Export (typeof (IDebuggerWindow))]
	public class ExecutionWindow : DebuggerWindow
	{
		[Import]
		public ISourceNavigator SourceNavigator { get; set; }
		private readonly IDebuggerSession session;
		private readonly IExecutionProvider executionProvider;

		ILocation lastStop;

		[ImportingConstructor]
		public ExecutionWindow (IDebuggerSession session, IExecutionProvider executionProvider)
		{
			this.session = session;
			this.executionProvider = executionProvider;
			this.executionProvider.Break += location =>
				{
					if (lastStop == location) {
						executionProvider.Resume ();
						return;
					}
					lastStop = location;
					SourceNavigator.ShowSourceLocation (location);
				};
		}

		public override void OnGUI()
		{
			GUI.enabled = session.Active && !executionProvider.Running;
			if (GUILayout.Button("Continue"))
				executionProvider.Resume ();
			if (GUILayout.Button("Step Over"))
				executionProvider.Step (StepType.Over);
			if (GUILayout.Button("Step In"))
				executionProvider.Step (StepType.Into);
			if (GUILayout.Button("Step Out"))
				executionProvider.Step (StepType.Out);

			GUILayout.FlexibleSpace();
			GUI.enabled = true;

			//if (session.Active) {
			//    if (GUILayout.Button("Stop"))
			//        session.Stop ();
			//}
			//else
			//{
			//    if (GUILayout.Button("Start"))
			//        session.Start ();
			//}
		}

		public string Title
		{
			get { return "Controls"; }
		}
	}
}
