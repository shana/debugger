using System;
using Debugger.Backend;

namespace Debugger
{
	public class Breakpoint : IBreakpoint
	{
		public ILocation Location { get; set; }
		public bool Enabled { get; private set; }
		public static event Action<IBreakpoint> OnEnable;
		public static event Action<IBreakpoint> OnDisable;

		public Breakpoint (ILocation location)
		{
			Location = location;
		}
	
		public T Unwrap<T> () where T : class
		{
			return null;
		}

		public void Enable ()
		{
			Enabled = true;
			if (OnEnable != null)
				OnEnable (this);
		}

		public void Disable ()
		{
			Enabled = false;
			if (OnDisable != null)
				OnDisable (this);
		}

	}
}
