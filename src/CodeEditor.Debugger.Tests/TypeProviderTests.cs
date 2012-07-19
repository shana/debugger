using CodeEditor.Debugger.Backend;
using CodeEditor.Debugger.Implementation;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Debugger.Tests
{
	[TestFixture]
	public class TypeProviderTests
	{
		private Mock<IDebuggerSession> _session;
		private TypeMirrorProvider _typeMirrorProvider;

		[SetUp]
		public void Setup()
		{
			_session = new Mock<IDebuggerSession>();
			_typeMirrorProvider = new TypeMirrorProvider(_session.Object);
		}

		[Test]
		public void LoadedTypes_EmptyOnStartup()
		{
			CollectionAssert.IsEmpty(_typeMirrorProvider.LoadedTypesMirror);
		}

		[Test]
		public void OneLoadedType_LoadedTypes_HasThatType()
		{
			var debugType = new Mock<ITypeMirror>();
			_session.Raise(s => s.TypeLoaded += null, debugType.Object);
			CollectionAssert.AreEquivalent(new[] { debugType.Object }, _typeMirrorProvider.LoadedTypesMirror);
		}

		[Test]
		public void TypeWhoseAssemblyUnloaded_LoadedTypes_IsEmpty()
		{
			var debugType = new Mock<ITypeMirror>();
			var debugAssembly = new Mock<IAssemblyMirror>();
			SetAssemblyForType(debugType, debugAssembly);

			_session.Raise(s => s.TypeLoaded += null, debugType.Object);
			_session.Raise(s => s.AssemblyUnloaded += null, debugAssembly.Object);
			
			CollectionAssert.IsEmpty(_typeMirrorProvider.LoadedTypesMirror);
		}

		private static void SetAssemblyForType(Mock<ITypeMirror> debugType, Mock<IAssemblyMirror> debugAssembly)
		{
			debugType.SetupGet(d => d.Assembly).Returns(debugAssembly.Object);
		}

		[Test]
		public void TypeLoaded_PublishesEvent()
		{
			var debugType = new Mock<ITypeMirror>();

			ITypeMirror loadedTypeMirror = null;
			_typeMirrorProvider.TypeLoaded += t => loadedTypeMirror = t;
			_session.Raise(s => s.TypeLoaded += null, debugType.Object);
			Assert.AreEqual(debugType.Object,loadedTypeMirror);
		}

		[Test]
		public void AssemblyUnload_Publishes_TypeUnloadEvents()
		{
			var debugType = new Mock<ITypeMirror>();
			var debugAssembly = new Mock<IAssemblyMirror>();
			SetAssemblyForType(debugType,debugAssembly);

			ITypeMirror unloadedTypeMirror = null;
			_typeMirrorProvider.TypeUnloaded += t => unloadedTypeMirror = t;
			_session.Raise(s => s.TypeLoaded += null, debugType.Object);
			_session.Raise(s => s.AssemblyUnloaded += null, debugAssembly.Object);

			Assert.AreEqual(debugType.Object, unloadedTypeMirror);
		}

	}
}
