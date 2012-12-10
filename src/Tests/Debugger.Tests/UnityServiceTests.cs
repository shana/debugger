using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CodeEditor.Remoting;

namespace CodeEditor.Debugger.Tests
{
	class DebuggerServiceClient : Client
	{
		public DebuggerServiceClient()
		{
			Port = 12346;
		}

		public string SendRequest(string message)
		{
			var req = Serializer.PackRequest(RequestType.Service, 1, message);
			var response = SendRequest(req);
			return Serializer.Unpack<string>(response);
		}
	}

	[TestFixture]
	class UnityServiceTests
	{
		private DebuggerServiceClient _client;

		[SetUp]
		public void Setup()
		{
			_client = new DebuggerServiceClient();
			_client.Start();
		}

		[TearDown]
		public void Teardown()
		{
			_client.Stop();
		}

		[Test]
		public void PingTest()
		{
			var response = _client.SendRequest("Ping");
			Assert.AreEqual("Assets/AScript.cs", response);
		}
	}
}
