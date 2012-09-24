using System;
using System.Collections.Generic;
using CodeEditor.Composition;
using CodeEditor.Remoting;
using UnityEngine;

namespace CodeEditor.Debugger.Unity.Standalone
{
	// keep this in sync with the server
	enum ServiceRequestType : ushort
	{
		Unknown = 0,
		Sources = 1
	}

	[Export (typeof (ISourcesProvider))]
	class ServiceClient : Client, ISourcesProvider
	{
		private bool _refreshing = false;
		private bool _started = false;
		private List<string> _sources = new List<string>();
		public IList<string> Sources
		{
			get { return _sources; }
		}

		public ServiceClient()
		{
			Port = 12346;
		}

		public override void Start ()
		{
			if (!_started)
			{
				_started = true;
				base.Start();
			}
		}

		public override void Stop ()
		{
			if (_started)
			{
				_started = false;
				base.Stop ();
			}
		}

		public void StartRefreshingSources(EventHandler callback, object state)
		{
			Start();
			_refreshing = true;
			RefreshSources(callback, state);
		}

		public void StopRefreshingSources()
		{
			_refreshing = false;
			Stop ();
		}

		void RefreshSources (EventHandler callback, object state)
		{
			var req = RequestData.Create(RequestType.Service, (ushort)ServiceRequestType.Sources, null);
			SendRequestAsync (req, ar => OnRefreshSources (ar, callback), state);
			BeginReceive ();
		}
		
		void OnRefreshSources (IAsyncResult ar, EventHandler callback)
		{
			try
			{
				var response = ((AsyncRequestResult)ar).Response;

				var sources = Serializer.Unpack<string[]>(response);
				_sources = new List<string>(sources);
				if (callback != null)
					callback(this, null);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			if (_refreshing)
				RefreshSources (callback, ar.AsyncState);
		}

		public override string ToString ()
		{
			return "Service Client provider on port " + Port;
		}
	}
}
