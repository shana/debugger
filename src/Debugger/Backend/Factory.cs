using System;
using CodeEditor.Composition.Hosting;

namespace Debugger.Backend
{
	public static class LogProvider
	{
		public static event Action<string> Debug;
		public static event Action<string> Error;

		public static void Log (string format, params object[] args)
		{
			if (Debug != null)
				Debug (string.Format (format, args));
		}

		public static void LogError (string format, params object[] args)
		{
			if (Error != null)
				Error (string.Format (format, args));
		}

		public static void LogError (object value)
		{
			if (Error != null)
				Error (value == null ? "" : value.ToString ());
		}

		public static void WithErrorLogging (Action action)
		{
			try
			{
				action ();
			}
			catch (Exception e)
			{
				LogError (e);
			}
		}
	}

	public interface IFactory
	{
		void Initialize ();
	}

	public static class Factory
	{
		public static Func<IVirtualMachine> CreateVirtualMachine {get; private set;} 
		public static Func<ILocation, IBreakpoint> CreateBreakpoint {get; private set; }
		public static Func<IThreadMirror, IEventRequest> CreateStepRequest {get; private set; }
		public static Func<IEventRequest> CreateMethodEntryRequest {get; private set; }
		public static Func<IEventRequest> CreateMethodExitRequest {get; private set; }
		public static Func<string, int, ILocation> CreateLocation { get; private set; }

		static Factory ()
		{
			var compositionContainer = new CompositionContainer (new DirectoryCatalog (Environment.CurrentDirectory));
			compositionContainer.GetExportedValue<IFactory> ().Initialize ();
		}

		public static void Register (Func<IVirtualMachine> createVM)
		{
			CreateVirtualMachine = createVM;
		}

		public static void Register (
			Func<ILocation, IBreakpoint> createBreakpoint, 
			Func<IThreadMirror, IEventRequest> createStepRequest,
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
