using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeEditor.Debugger.Implementation;
using Moq;
using NUnit.Framework;

namespace CodeEditor.Debugger.Tests
{
	[TestFixture]
	public class BreakPointMediatorTests
	{
		private Mock<IDebuggerSession> _session;
		private Mock<IDebugBreakPointProvider> _breakPointProvider;
		private IDebugTypeProvider _typeProvider;

		[SetUp]
		public void SetUp()
		{
			_session = new Mock<IDebuggerSession>();
			_breakPointProvider = new Mock<IDebugBreakPointProvider>();
			_typeProvider = new DebugTypeProvider(_session.Object);
			new BreakpointMediator(_session.Object, _breakPointProvider.Object, _typeProvider);
		}

		[Test]
		public void WhenTypeLoadsMatchingExistingBreakPoint_CreatesBreakRequest()
		{
			AddBreakpoint(MockBreakPointFor("myfile.cs",3));
			RaiseTypeLoad(MockTypeWithMethodFrom("myfile.cs", 3));
			VerifyCreateBreakpointRequest("myfile.cs",3);
		}

		[Test]
		public void BreakPointGetsAddedMatchingAlreadyLoadedType_CreatesBreakRequest()
		{
			RaiseTypeLoad(MockTypeWithMethodFrom("myfile.cs",5));
			AddBreakpoint(MockBreakPointFor("myfile.cs",5));
			VerifyCreateBreakpointRequest("myfile.cs",5);
		}

		private void VerifyCreateBreakpointRequest(string expectedFile, int line)
		{
			_session.Verify(s => s.CreateBreakpointRequest(It.Is<IDebugLocation>(location => location.File==expectedFile && location.LineNumber == line)));
		}

		private void AddBreakpoint(Mock<IBreakPoint> breakpoint)
		{
			_breakPointProvider.Raise(bpp => bpp.BreakpointAdded += null, breakpoint.Object);
		}

		private void RaiseTypeLoad(Mock<IDebugType> debugType)
		{
			_session.Raise(tp => tp.TypeLoaded += null, debugType.Object);
		}

		private static Mock<IDebugType> MockTypeWithMethodFrom(string sourceFile, int line)
		{
			var debugType = new Mock<IDebugType>();
			debugType.SetupGet(t => t.SourceFiles).Returns(new[] {sourceFile});

			var debugMethod = new Mock<IDebugMethod>();
			var debugLocation = new Mock<IDebugLocation>();
			debugLocation.SetupGet(l => l.File).Returns(sourceFile);
			debugLocation.SetupGet(l => l.LineNumber).Returns(line);
			debugMethod.SetupGet(m => m.Locations).Returns(new[] {debugLocation.Object});
			debugType.SetupGet(t => t.Methods).Returns(new[] {debugMethod.Object});

			return debugType;
		}

		private static Mock<IBreakPoint> MockBreakPointFor(string file, int line)
		{
			var breakpoint = new Mock<IBreakPoint>();
			breakpoint.SetupGet(bp => bp.File).Returns(file);
			breakpoint.SetupGet(bp => bp.LineNumber).Returns(line);
			return breakpoint;
		}
	}
}
