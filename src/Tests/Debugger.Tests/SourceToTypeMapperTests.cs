using Debugger.Backend;
using Moq;
using NUnit.Framework;

namespace Debugger.Tests
{
	[TestFixture]
	public class SourceToTypeMapperTests : BaseDebuggerSessionTest
	{
		[SetUp]
		public void Setup ()
		{
			vm.Start ();
			session.Start ();
			vm.LoadAssembly (typeof(type1).Assembly.Location);
		}

		[TearDown]
		public void Teardown ()
		{
			session.Stop ();
		}

		[Test]
		public void NoTypesForUnknownFile ()
		{
			var types = typeProvider.TypesFor ("somenonexistingfile.cs");
			CollectionAssert.IsEmpty (types);
		}

		[Test]
		public void TypeIsFoundByItsSourceFile ()
		{
			var types = typeProvider.TypesFor ("type1.cs");
			CollectionAssert.AreEquivalent (typeProvider.LoadedTypes, types);
		}
	}
}
