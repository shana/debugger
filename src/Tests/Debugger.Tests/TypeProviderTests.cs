using Debugger.Backend;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace Debugger.Tests
{
	[TestFixture]
	public class TypeProviderTests : BaseDebuggerSessionTest
	{
		[SetUp]
		public void Setup ()
		{
			vm.Start ();
			session.Start ();
		}

		[TearDown]
		public void Teardown ()
		{
			session.Stop ();
		}

		[Test]
		public void LoadedTypes_EmptyOnStartup ()
		{
			vm.Reset ();
			CollectionAssert.IsEmpty (typeProvider.LoadedTypes);
		}

		[Test]
		public void OneLoadedType_LoadedTypes_HasThatType ()
		{
			vm.Reset ();
			vm.LoadAssembly (typeof(type1).Assembly.Location);
			Assert.AreEqual (1, typeProvider.LoadedTypes.Count);
			CollectionAssert.AreEquivalent (typeProvider.LoadedTypes.Select (t => t.FullName), new string[] { typeof (type1).FullName });
		}

		[Test]
		public void TypeWhoseAssemblyUnloaded_LoadedTypes_IsEmpty ()
		{
			vm.Reset ();
			vm.LoadAssembly (typeof(type1).Assembly.Location);
			vm.UnloadAssembly (typeof (type1).Assembly.FullName);
			CollectionAssert.IsEmpty (typeProvider.LoadedTypes);
		}

		[Test]
		public void TypeLoaded_PublishesEvent ()
		{
			vm.Reset ();
			ITypeMirror loadedType = null;
			typeProvider.TypeLoaded += mirror => loadedType = mirror;
			vm.LoadAssembly (typeof(type1).Assembly.Location);
			typeProvider.TypeLoaded += null;
			Assert.IsNotNull (loadedType);
		}

		[Test]
		public void AssemblyUnload_Publishes_TypeUnloadEvents ()
		{
			vm.Reset ();
			vm.LoadAssembly (typeof(type1).Assembly.Location);
			ITypeMirror unloadedType = null;
			typeProvider.TypeUnloaded += mirror => unloadedType = mirror;
			vm.UnloadAssembly (typeof (type1).Assembly.FullName);
			typeProvider.TypeLoaded += null;
			Assert.IsNotNull (unloadedType);
		}
	}
}
