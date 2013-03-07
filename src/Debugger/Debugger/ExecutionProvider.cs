using System;
using System.Collections.Generic;
using CodeEditor.Composition;
using System.Linq;
using Debugger.Backend;
using Debugger.Backend.Event;

namespace Debugger
{
	[Export (typeof (IExecutionProvider))]
	public class ExecutionProvider : IExecutionProvider
	{
		private static object obj = new object ();
		private ILocation currentLocation;
		private IThreadMirror currentThread;
		private readonly IVirtualMachine vm;
		private bool running;
		private readonly Dictionary<IThreadMirror, IEventRequest> requests = new Dictionary<IThreadMirror, IEventRequest> ();

		public event Action<IThreadMirror> Suspended;
		public event Action<ILocation> Break;

		public ILocation Location { get { return currentLocation ?? Debugger.Location.Default; } }

		public bool Running { get { lock (obj) { return running; } } }
		public IThreadMirror CurrentThread { get { return currentThread; } }

		[ImportingConstructor]
		public ExecutionProvider (IVirtualMachine vm)
		{
			this.vm = vm;
			this.vm.VMSuspended += VMSuspended;
			this.vm.Stepped += OnStepped;
			this.vm.ThreadStarted += OnThreadStarted;
			this.vm.BreakpointHit += OnBreakpointHit;
			currentLocation = null;
			currentThread = null;
			running = true;
		}

		private void OnBreakpointHit (IBreakpointEvent ev)
		{
			vm.Suspend (ev);
			if (Break != null)
				Break (currentLocation);
		}

		private void OnThreadStarted (IEvent ev)
		{
			if (ev.State == State.Unload)
			{
				var reqs = requests.Keys.Where (t => t.Equals (ev.Thread));
				foreach (var t in reqs)
				{
					requests[t].Disable ();
					requests.Remove (t);
				}
			}
		}

		private void VMSuspended (IEvent suspendingEvent)
		{
			lock (obj)
			{
				running = false;
				if (Suspended != null)
					Suspended (suspendingEvent.Thread);

				currentThread = suspendingEvent.Thread;
				var frames = currentThread.GetFrames ();
				currentLocation = frames.Count == 0 ? new Location ("", 0) : frames[0].Location;
				//foreach (var frame in frames)
				//{
				//    LogProvider.Log ("frame:" + frame.Location.LineNumber);
				//}
			}
		}

		public void Resume ()
		{
			lock (obj)
			{
				if (!running)
				{
					running = true;
				}
			}
			vm.Resume ();
		}

		public void Step (StepType stepType)
		{
			lock (obj)
			{
				if (!running)
				{
					IEventRequest request;
					if (!requests.TryGetValue (currentThread, out request))
					{
						request = Factory.CreateStepRequest (currentThread, stepType);
						requests.Add (currentThread, request);
					}
					request.Enable ();
				}
			}
			Resume ();
		}

		private void OnStepped (IEvent ev)
		{
			vm.Suspend (ev);
			ev.Request.Disable ();

			if (Break != null)
				Break (currentLocation);
		}
	}
}
