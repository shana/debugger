using Debugger.Backend;

namespace Debugger.DummyProviders
{

	class Breakpoint : EventRequest, IBreakpoint
	{
		public override void Enable ()
		{
			Enabled = true;
			base.Enable ();
		}

		public override void Disable ()
		{
			Enabled = false;
			base.Disable ();
		}

		public ILocation Location { get; set; }
		public bool Enabled { get; set; }
	}
}
