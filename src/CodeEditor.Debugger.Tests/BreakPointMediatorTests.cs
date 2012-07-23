using CodeEditor.Debugger.Backend;
using CodeEditor.Debugger.Implementation;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Debugger.Tests
{
	[TestFixture]
	public class BreakPointMediatorTests
	{
		private Mock<IDebuggerSession> _session;
		private BreakpointProvider _breakPointProvider;
		private ITypeMirrorProvider _typeMirrorProvider;
		private Mock<IBreakpointEventRequestFactory> _breakpointEventRequestFactory;
		private Mock<IBreakpointEventRequest> _breakRequest;

		[SetUp]
		public void SetUp()
		{
			_session = new Mock<IDebuggerSession>();
			_breakPointProvider = new BreakpointProvider();
			_typeMirrorProvider = new TypeMirrorProvider(_session.Object);
			_breakpointEventRequestFactory = new Mock<IBreakpointEventRequestFactory>(MockBehavior.Strict);
			_breakRequest = new Mock<IBreakpointEventRequest>();
			new BreakpointMediator_old(_breakPointProvider, _typeMirrorProvider, _breakpointEventRequestFactory.Object);
		}

		[Test]
		public void WhenTypeLoadsMatchingExistingBreakPoint_CreatesBreakRequest()
		{
			SetupBreakEventRequestFactory("myfile.cs", 3, _breakRequest.Object);
			AddBreakpoint("myfile.cs",3);
			RaiseTypeLoad(MockTypeWithMethodFrom("myfile.cs", 3));

			_breakRequest.Verify(r=>r.Enable());
			VerifyMocks();
		}

		[Test]
		public void BreakPointGetsAddedMatchingAlreadyLoadedType_CreatesBreakRequest()
		{
			SetupBreakEventRequestFactory("myfile.cs", 5, _breakRequest.Object);
			RaiseTypeLoad(MockTypeWithMethodFrom("myfile.cs",5));
			AddBreakpoint("myfile.cs",5);

			_breakRequest.Verify(r => r.Enable());
			VerifyMocks();
		}

		[Test]
		public void TypeLoadWithMethodInSameFileButDifferentLine_DoesNotCreateBreakRequest()
		{
			AddBreakpoint("myfile.cs", 5);
			RaiseTypeLoad(MockTypeWithMethodFrom("myfile.cs", 10));
		}

		private void SetupBreakEventRequestFactory(string file, int lineNumber, IBreakpointEventRequest breakRequestToCreate)
		{
			_breakpointEventRequestFactory
				.Setup(f => f.Create(It.Is<ILocation>(location => location.File == file && location.LineNumber == lineNumber)))
				.Returns(breakRequestToCreate);
		}

		private void VerifyMocks()
		{
			_breakpointEventRequestFactory.VerifyAll();
			_session.VerifyAll();
			_breakRequest.VerifyAll();
		}

		private void AddBreakpoint(string file, int line)
		{
			_breakPointProvider.ToggleBreakPointAt(file,line);
		}

		private void RaiseTypeLoad(Mock<ITypeMirror> debugType)
		{
			_session.Raise(tp => tp.TypeLoaded += null, debugType.Object);
		}

		private static Mock<ITypeMirror> MockTypeWithMethodFrom(string sourceFile, int line)
		{
			var debugType = new Mock<ITypeMirror>();
			debugType.SetupGet(t => t.SourceFiles).Returns(new[] {sourceFile});

			var debugMethod = new Mock<IMethodMirror>();
			var debugLocation = new Mock<ILocation>();
			debugLocation.SetupGet(l => l.File).Returns(sourceFile);
			debugLocation.SetupGet(l => l.LineNumber).Returns(line);
			debugMethod.SetupGet(m => m.Locations).Returns(new[] {debugLocation.Object});
			debugType.SetupGet(t => t.Methods).Returns(new[] {debugMethod.Object});

			return debugType;
		}
	}
}
