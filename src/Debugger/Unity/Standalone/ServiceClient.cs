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
		private List<string> _sources = new List<string>();
		public IList<string> Sources
		{
			get { return _sources; }
		}

		public ServiceClient()
		{
			Port = 12346;
		}

		public void StartRefreshingSources (Action<object> callback, object state)
		{
			Start();
			_refreshing = true;
			RequestSourceRefresh(callback, state);
		}

		public void StopRefreshingSources()
		{
			_refreshing = false;
			Stop ();
		}

		void RequestSourceRefresh (Action<object> callback, object state)
		{
			var req = RequestData.Create(RequestType.Service, (ushort)ServiceRequestType.Sources, null);
			SendRequestAsync (req, ar => OnRefreshSources (ar, callback), state);
			BeginReceive ();
		}
		
		void OnRefreshSources (IAsyncResult ar, Action<object> callback)
		{
			try
			{
				var response = ((AsyncRequestResult)ar).Response;
				var sources = Serializer.Unpack<string[]>(response);
				_sources = new List<string>(sources);
				if (callback != null)
					callback(ar.AsyncState);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		protected override bool HandleData (System.Net.EndPoint sender, byte[] data)
		{
			base.HandleData (sender, data);
			return _refreshing;
		}

		public override string ToString ()
		{
			return "Service Client provider on port " + Port;
		}
	}
}
