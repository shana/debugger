using System;
using System.Collections.Generic;
using Debugger.Backend;

namespace Debugger
{
	public interface IBreakpointProvider
	{
		IList<IBreakpoint> Breakpoints { get; }

		/// <summary>
		/// Returns the list of existing breakpoints.
		/// If bound is true, then returns the list of
		/// breakpoints which have been successfully bound
		/// in the VM.
		/// </summary>
		/// <param name="bound"></param>
		/// <returns></returns>
		IEnumerable<IBreakpoint> GetBreakpoints (bool bound);

		/// <summary>
		/// Returns the breakpoint set at the provided user location.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="line"></param>
		/// <returns></returns>
		IBreakpoint GetBreakpointAt (string file, int line);
		void ToggleBreakpointAt (string file, int line);

		IBreakpoint AddBreakpoint (string file, int line);
		bool RemoveBreakpoint (string file, int line);
		bool RemoveBreakpoint (IBreakpoint breakpoint);

		bool IsBound (IBreakpoint breakpoint);

		/// <summary>
		/// Returns the actual runtime location for this breakpoint. If
		/// this breakpoint represents more than one location (if the runtime supports it),
		/// returns the first location.
		/// </summary>
		/// <param name="breakpoint"></param>
		/// <returns></returns>
		ILocation GetBoundLocation (IBreakpoint breakpoint);

		/// <summary>
		/// Returns all the actual runtime locations for this breakpoint.
		/// </summary>
		/// <param name="breakpoint"></param>
		/// <returns></returns>
		IEnumerable<ILocation> GetBoundLocations (IBreakpoint breakpoint);

		/// <summary>
		/// Handy accessor for accessing breakpoints by index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		IBreakpoint this[int index] { get; }

		int IndexOf (IBreakpoint breakpoint);

		event Action<IBreakpoint, ILocation> BreakpointBound;
		event Action<IBreakpoint, ILocation> BreakpointUnbound;
	}
}
