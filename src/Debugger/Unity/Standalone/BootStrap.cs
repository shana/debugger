using UnityEngine;

namespace Debugger.Unity.Standalone
{
	public class BootStrap : MonoBehaviour
	{
		private void Start()
		{
			Main.Start();
		}

		private void Update()
		{
			Main.Update();
		}

		private void OnGUI()
		{
			Main.OnGUI();
		}

		private void FixedUpdate()
		{
			Main.FixedUpdate();
		}

		private void OnApplicationQuit()
		{
			Main.OnApplicationQuit();
		}
	}
}
