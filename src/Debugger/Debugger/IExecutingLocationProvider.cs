using Debugger.Backend;

namespace Debugger
{
	public interface IExecutingLocationProvider
	{
		ILocation Location { get; }
	}
}
