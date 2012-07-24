namespace CodeEditor.Debugger.Implementation
{
	class BreakPoint : IBreakPoint
	{
		private readonly Location _location;

		public BreakPoint(Location location)
		{
			_location = location;
		}

		public ILocation Location { get { return _location;  } }
	}
}