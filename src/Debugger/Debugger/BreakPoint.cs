using System;
using Debugger.Backend;

namespace Debugger
{
	public class Breakpoint : EventRequest, IBreakpoint
	{
		public ILocation Location { get; set; }
		public bool Enabled { get; private set; }
		public static event Action<IBreakpoint> OnEnable;
		public static event Action<IBreakpoint> OnDisable;

		public Breakpoint (ILocation location)
		{
			Location = location;
			Enabled = true;
		}
	
		public override void Enable ()
		{
			if (Enabled)
				return;

			Enabled = true;
			if (OnEnable != null)
				OnEnable (this);
		}

		public override void Disable ()
		{
			if (!Enabled)
				return;

			Enabled = false;
			if (OnDisable != null)
				OnDisable (this);
		}
	}
}
