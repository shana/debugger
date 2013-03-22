using System;
using CodeEditor.Composition.Hosting;

namespace Debugger.Backend
{
	public interface IFactory
	{
		void Initialize ();
	}

	public static class Factory
	{
		public static Func<IVirtualMachine> CreateVirtualMachine {get; private set;} 
		public static Func<ILocation, IBreakpoint> CreateBreakpoint {get; private set; }
		public static Func<IThreadMirror, StepType, IEventRequest> CreateStepRequest {get; private set; }
		public static Func<IEventRequest> CreateMethodEntryRequest {get; private set; }
		public static Func<IEventRequest> CreateMethodExitRequest {get; private set; }
		public static Func<string, int, ILocation> CreateLocation { get; private set; }

		static Factory ()
		{
			//var compositionContainer = new CompositionContainer (new DirectoryCatalog (Environment.CurrentDirectory));
			//compositionContainer.GetExportedValue<IFactory> ().Initialize ();
		}

		public static void Register (Func<IVirtualMachine> createVM)
		{
			CreateVirtualMachine = createVM;
		}

		public static void Register (
			Func<ILocation, IBreakpoint> createBreakpoint, 
			Func<IThreadMirror, StepType, IEventRequest> createStepRequest,
			Func<IEventRequest> createMethodEntryRequest,
			Func<IEventRequest> createMethodExitRequest,
			Func<string, int, ILocation> createLocation)
		{
			CreateBreakpoint = createBreakpoint;
			CreateStepRequest = createStepRequest;
			CreateMethodEntryRequest = createMethodEntryRequest;
			CreateMethodExitRequest = createMethodExitRequest;
			CreateLocation = createLocation;
		}
	}
}
