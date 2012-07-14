using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
	void Start ()
	{
		CodeEditor.Debugger.Unity.Standalone.Main.Start ();
	}
	
	void OnGUI ()
	{
		CodeEditor.Debugger.Unity.Standalone.Main.OnGUI ();
	}
}
