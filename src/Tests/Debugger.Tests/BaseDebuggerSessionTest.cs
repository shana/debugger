using System;
using System.IO;
using System.Reflection;
using CodeEditor.Composition.Hosting;
using Debugger.DummyProviders;

namespace Debugger.Tests
{
	public class BaseDebuggerSessionTest
	{
		protected IDebuggerSession session;
		protected ITypeProvider typeProvider;
		protected VirtualMachine vm;

		public BaseDebuggerSessionTest ()
		{
			var container = new CompositionContainer (new DirectoryCatalog (Environment.CurrentDirectory));
			session = container.GetExportedValue<IDebuggerSession> ();
			typeProvider = session.TypeProvider;
			typeProvider.AddFilter (Path.GetDirectoryName (typeof (type1).Assembly.Location));
			vm = session.VM as VirtualMachine;
		}
	}
}
