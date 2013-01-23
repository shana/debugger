using System;
using System.Collections.Generic;
using System.Linq;
using CodeEditor.Composition;
using Debugger.Backend;
using Debugger.Backend.Event;

namespace Debugger
{
	[Export (typeof (ITypeProvider))]
	class TypeProvider : ITypeProvider
	{
		private readonly IVirtualMachine vm;
		private readonly List<ITypeMirror> types = new List<ITypeMirror> ();
		private readonly List<string> filter = new List<string> ();

		public event Action<ITypeEvent, ITypeMirror> TypeLoaded;
		public event Action<ITypeMirror> TypeUnloaded;

		[ImportingConstructor]
		internal TypeProvider (IVirtualMachine vm)
		{
			this.vm = vm;
			this.vm.OnType += OnType;
			this.vm.OnAssembly += OnAssembly;
		}

		public ITypeMirror[] LoadedTypes
		{
			get { return types.ToArray (); }
		}

		private void OnType (ITypeEvent ev)
		{
			if (filter.Count > 0 && !filter.Contains (ev.Type.Name))
				return;

			if (TypeLoaded != null)
			{
				TypeLoaded (ev, ev.Type);
				if (!ev.Cancel)
					types.Add (ev.Type);
			}
		}

		private void OnAssembly (IAssemblyEvent ev)
		{
			if (ev.State == State.Unload)
			{
				var asm = ev.Assembly;
				var unloadedTypes = types.Where (t => t.Assembly.Equals (asm)).ToArray ();
				types.RemoveAll (t => unloadedTypes.Contains (t));
				if (TypeUnloaded != null)
				{
					foreach (var type in unloadedTypes)
						TypeUnloaded (type);
				}
			}
		}

		public void AddFilter (params string[] typeNames)
		{
			foreach (var typeName in typeNames)
			{
				if (!filter.Contains (typeName))
					filter.Add (typeName);
			}
		}

		public void RemoveFilter (params string[] typeNames)
		{
			foreach (var typeName in typeNames)
			{
				if (filter.Contains (typeName))
					filter.Remove (typeName);
			}
		}

		public void RemoveAllFilters ()
		{
			filter.Clear ();
		}
	}
}
