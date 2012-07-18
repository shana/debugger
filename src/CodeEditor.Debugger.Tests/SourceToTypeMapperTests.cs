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
	public class SourceToTypeMapperTests
	{
		private Mock<IDebuggerSession> _session;
		private Mock<IDebugTypeProvider> _typeProvider;
		private SourceToTypeMapper _mapper;

		[SetUp]
		public void Setup()
		{
			_session = new Mock<IDebuggerSession>();
			_typeProvider = new Mock<IDebugTypeProvider>();
			_mapper = new SourceToTypeMapper(_session.Object, _typeProvider.Object);			
		}

		[Test]
		public void NoTypesForUnknownFile()
		{
			var types = _mapper.TypesFor("somenonexistingfile.cs");
			CollectionAssert.IsEmpty(types);
		}

		[Test]
		public void TypeWithMethodInFileIsFound()
		{
			var debugType = new Mock<IDebugType>();
			debugType.SetupGet(t => t.SourceFiles).Returns(new[] { "myfile.cs" });

			_typeProvider.Raise(tp => tp.TypeLoaded += null, debugType.Object);

			var types = _mapper.TypesFor("myfile.cs");
			CollectionAssert.AreEquivalent(new[]{debugType.Object}, types);
		}
	}
}
