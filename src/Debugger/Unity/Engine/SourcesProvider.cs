using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeEditor.Composition;

namespace Debugger.Unity.Engine
{
	public interface ISourcesProvider
	{
		IList<string> SourceFiles { get; }

		event Action<string> FileChanged;
		event Action SourcesChanged;

		void Start ();
		void Stop ();
	}


	[Export (typeof (ISourcesProvider))]
	class SourcesProvider : ISourcesProvider
	{
		FileSystemWatcher fsw = null;
		IList<string> sources = new List<string> ();

		[Import]
		public ITypeProvider TypeProvider { get; set; }

		public string Path { get { return TypeProvider.BasePath; } }
		public IList<string> SourceFiles { get { return sources; } }

		public event Action<string> FileChanged;
		public event Action SourcesChanged;

		public void Start ()
		{
			RescanFS (null, null);
			fsw = new FileSystemWatcher (Path) { IncludeSubdirectories = true };
			fsw.Changed += FSChanged;
			fsw.Created += RescanFS;
		}

		public void Stop ()
		{
			fsw.Changed -= FSChanged;
			fsw.Created -= RescanFS;
			fsw = null;
		}

		string[] extensions = { ".cs", ".js" };
		private void RescanFS (object sender, FileSystemEventArgs e)
		{
			sources = Directory.GetFiles (Path, "*", SearchOption.AllDirectories).Where (f => extensions.Contains (System.IO.Path.GetExtension (f))).ToArray ();
			if (SourcesChanged != null)
				SourcesChanged ();
		}

		private void FSChanged (object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			if (FileChanged != null) {
				var ext = System.IO.Path.GetExtension (fileSystemEventArgs.FullPath);
				if (ext == "cs" || ext == "js")
					FileChanged (fileSystemEventArgs.FullPath);
			}
		}
	}

	/*
		// keep this in sync with the server
		enum ServiceRequestType : ushort
		{
			Unknown = 0,
			Sources = 1
		}

	 * [Export (typeof (ISourcesProvider))]
		class ServiceClient : Client, ISourcesProvider
		{
			private bool refreshing = false;
			Action<object, object> callback;
			object state;

			private List<string> sources = new List<string> ();
			public IList<string> Sources
			{
				get { return sources; }
			}

			public ServiceClient ()
			{
				Port = 12346;
			}

			private void OnConnected ()
			{
				RequestSourceRefresh (callback, state);
			}

			public void StartRefreshingSources (Action<object, object> callback, object state)
			{
				OnConnect += OnConnected;
				this.callback = callback;
				this.state = state;
				Start ();
				refreshing = true;
			}

			public void StopRefreshingSources ()
			{
				refreshing = false;
				Stop ();
			}

			void RequestSourceRefresh (Action<object, object> callback, object state)
			{
				var req = RequestData.Create (RequestType.Service, (ushort)ServiceRequestType.Sources, null);
				SendRequestAsync (req, ar => OnRefreshSources (ar, callback), state);
				BeginReceive ();
			}

			void OnRefreshSources (IAsyncResult ar, Action<object, object> callback)
			{
				Console.WriteLine ("OnRefreshSources ");
				try
				{
					var response = ((AsyncRequestResult)ar).Response;
					var sources = Serializer.Unpack<string[]> (response);
					this.sources = new List<string> (sources);
					if (callback != null)
						callback (this.sources, ar.AsyncState);
				}
				catch (Exception ex)
				{
					Console.WriteLine (ex);
				}
			}

			protected override bool HandleData (System.Net.EndPoint sender, byte[] data)
			{
				base.HandleData (sender, data);
				return refreshing;
			}

			public override string ToString ()
			{
				return "Service Client provider on port " + Port;
			}
		}
	*/
}
