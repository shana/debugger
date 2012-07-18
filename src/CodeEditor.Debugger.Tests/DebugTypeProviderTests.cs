using CodeEditor.Debugger.Implementation;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Debugger.Tests
{
	[TestFixture]
	public class DebugTypeProviderTests
	{
		private Mock<IDebuggerSession> _session;
		private DebugTypeProvider _debugTypeProvider;

		[SetUp]
		public void Setup()
		{
			_session = new Mock<IDebuggerSession>();
			_debugTypeProvider = new DebugTypeProvider(_session.Object);
		}

		[Test]
		public void LoadedTypes_EmptyOnStartup()
		{
			CollectionAssert.IsEmpty(_debugTypeProvider.LoadedTypes);
		}

		[Test]
		public void OneLoadedType_LoadedTypes_HasThatType()
		{
			var debugType = new Mock<IDebugType>();
			_session.Raise(s => s.TypeLoaded += null, debugType.Object);
			CollectionAssert.AreEquivalent(new[] { debugType.Object }, _debugTypeProvider.LoadedTypes);
		}

		[Test]
		public void TypeWhoseAssemblyUnloaded_LoadedTypes_IsEmpty()
		{
			var debugType = new Mock<IDebugType>();
			var debugAssembly = new Mock<IDebugAssembly>();
			SetAssemblyForType(debugType, debugAssembly);

			_session.Raise(s => s.TypeLoaded += null, debugType.Object);
			_session.Raise(s => s.AssemblyUnloaded += null, debugAssembly.Object);
			
			CollectionAssert.IsEmpty(_debugTypeProvider.LoadedTypes);
		}

		private static void SetAssemblyForType(Mock<IDebugType> debugType, Mock<IDebugAssembly> debugAssembly)
		{
			debugType.SetupGet(d => d.Assembly).Returns(debugAssembly.Object);
		}

		[Test]
		public void TypeLoaded_PublishesEvent()
		{
			var debugType = new Mock<IDebugType>();

			IDebugType loadedType = null;
			_debugTypeProvider.TypeLoaded += t => loadedType = t;
			_session.Raise(s => s.TypeLoaded += null, debugType.Object);
			Assert.AreEqual(debugType.Object,loadedType);
		}

		[Test]
		public void AssemblyUnload_Publishes_TypeUnloadEvents()
		{
			var debugType = new Mock<IDebugType>();
			var debugAssembly = new Mock<IDebugAssembly>();
			SetAssemblyForType(debugType,debugAssembly);

			IDebugType unloadedType = null;
			_debugTypeProvider.TypeUnloaded += t => unloadedType = t;
			_session.Raise(s => s.TypeLoaded += null, debugType.Object);
			_session.Raise(s => s.AssemblyUnloaded += null, debugAssembly.Object);

			Assert.AreEqual(debugType.Object, unloadedType);
		}

	}
}
