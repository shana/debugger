using System;
using System.Collections.Generic;
using CodeEditor.Composition;
using Debugger.Backend;
using Mono.Cecil;
using System.Linq;
using Mono.Cecil.Pdb;

namespace Debugger.DummyProviders
{
	[Export(typeof(IVirtualMachine))]
	public class VirtualMachine : BaseMirror, IVirtualMachine
	{
		readonly List<IAssemblyMirror> assemblies = new List<IAssemblyMirror> ();
		public IList<IAssemblyMirror> Assemblies { get { return assemblies; } }
		public IList<IAssemblyMirror> RootAssemblies { get { return assemblies; } }
		public IList<IThreadMirror> Threads {
			get { return new List<IThreadMirror> () { new ThreadMirror () { Id = 1, Name = "Main" } }; }
		}

		public event Action<IEvent> VMStateChanged;
		public event Action<IEvent> VMSuspended;
		public event Action<IEvent> AppDomainLoaded;
		public event Action<IEvent> AppDomainUnloaded;
		public event Action<IEvent> ThreadStarted;
		public event Action<IEvent> ThreadStopped;
		public event Action<IAssemblyEvent> AssemblyLoaded;
		public event Action<IAssemblyEvent> AssemblyUnloaded;
		public event Action<ITypeEvent> TypeLoaded;
		public event Action<IBreakpointEvent> BreakpointHit;
		public event Action<IEvent> Stepped;

		public void Attach (int port)
		{
		}

		public void Detach ()
		{

		}

		public void Suspend ()
		{
			if (VMSuspended != null)
				VMSuspended (null);
		}

		public void Resume ()
		{

		}

		#region backend
		bool isRunning = false;

		public void Start ()
		{
			isRunning = true;
		}

		public void Stop ()
		{
			if (!isRunning)
				return;

			if (AssemblyUnloaded != null)
			{
				foreach (var assembly in assemblies)
				{
					AssemblyUnloaded (new AssemblyEvent (assembly));
				}
			}
		}

		public void DoSuspend ()
		{
			Suspend ();
		}

		public void Reset ()
		{
			Stop ();
			Start ();

		}

		public void DoBreakpointHit (string file, int line, IMethodMirror method)
		{
			if (BreakpointHit != null)
				BreakpointHit (new BreakpointEvent (new Location (file, line), method));
		}

		public void DoStep (string file, int line)
		{
			if (Stepped != null)
				Stepped (new Event ());
		}

		public void LoadAssembly (string assemblyPath)
		{
			var readerParameters = new ReaderParameters { ReadSymbols = true };
			var def = AssemblyDefinition.ReadAssembly (assemblyPath, readerParameters);
			
			var assembly = new AssemblyMirror ();
			assembly.FullName = def.FullName;
			assembly.Metadata = def;
			assemblies.Add (assembly);

			if (AssemblyLoaded != null)
				AssemblyLoaded (new AssemblyEvent (assembly));

			if (TypeLoaded == null)
				return;

			foreach (var t in def.MainModule.Types.Where (x => x.FullName != "<Module>"))
			{
				var type = new TypeMirror ();
				type.Assembly = assembly;
				type.FullName = t.FullName;
				type.Name = t.Name;
				type.MetadataToken = t.MetadataToken.ToInt32 ();
				type.Metadata = t;
				TypeLoaded (new TypeEvent (type));
			}
		}

		public void UnloadAssembly (string assemblyFullName)
		{
			var assembly = assemblies.FirstOrDefault (a => a.FullName == assemblyFullName);
			if (assembly == null)
				return;
			assemblies.Remove (assembly);
			if (AssemblyUnloaded != null)
				AssemblyUnloaded (new AssemblyEvent (assembly));
		}
	}

		#endregion
}
