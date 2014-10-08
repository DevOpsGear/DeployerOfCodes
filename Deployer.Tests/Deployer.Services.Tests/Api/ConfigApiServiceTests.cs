using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Deployer.Services.Api;
using Deployer.Services.Api.Interfaces;
using Deployer.Services.Builders;
using Deployer.Services.Config.Interfaces;
using Deployer.Services.Models;
using Json.NETMF;
using Moq;
using NUnit.Framework;

namespace Deployer.Tests.Api
{
	[TestFixture]
	internal class ConfigApiServiceTests
	{
		private Mock<IApiReadBody> _readBody;
		private Mock<IApiSocket> _socket;
		private Mock<IConfigurationService> _config;
		private ConfigApiService _sut;
		private ApiRequest _req;
		private ProjectModel _projectOne;
		private ProjectModel _projectTwo;
		private string _jsonOne;
		private string _jsonTwo;

		[SetUp]
		public void BeforeEachTest()
		{
			_projectOne = new ProjectModel("proj-1", "Project One", "Subtitle One", 1, BuildServiceProvider.AppVeyor);
			_projectTwo = new ProjectModel("proj-2", "Project Two", "Subtitle Two", 2, BuildServiceProvider.TeamCity);
			_jsonOne = @"{""Slug"":""proj-1"",""Subtitle"":""Subtitle One"",""Provider"":,""Title"":""Project One"",""Rank"":1}";
			_jsonTwo = @"{""Slug"":""proj-2"",""Subtitle"":""Subtitle Two"",""Provider"":,""Title"":""Project Two"",""Rank"":2}";

			_readBody = new Mock<IApiReadBody>();
			_socket = new Mock<IApiSocket>();
			_config = new Mock<IConfigurationService>();
			_req = new ApiRequest
				{
					Headers = new Hashtable(),
					GetArguments = new Hashtable(),
					Body = _readBody.Object,
					HttpMethod = "GET",
					Url = "projects/",
					Client = _socket.Object,
				};
			_sut = new ConfigApiService(_config.Object);
		}

		[Test]
		public void Can_only_respond_to_appropriate_endpoint()
		{
			_req.HttpMethod = "GET";
			_req.Url = "projects/";
			Assert.IsTrue(_sut.CanRespond(_req));

			_req.Url = "blerg/";
			Assert.IsFalse(_sut.CanRespond(_req));

			_req.Url = "schnarf/";
			Assert.IsFalse(_sut.CanRespond(_req));

			_req.Url = "projectiles/";
			Assert.IsFalse(_sut.CanRespond(_req));
		}

		[Test]
		public void Default_endpoint_supports_get()
		{
			_req.HttpMethod = "GET";
			_req.Url = "projects/";
			Assert.IsTrue(_sut.SendResponse(_req));
			_socket.Verify(x => x.Send200_OK("application/json", It.IsAny<int>()), Times.Once);
		}

		[Test]
		public void Default_endpoint_rejects_other_verbs()
		{
			_req.HttpMethod = "POST";
			Assert.IsTrue(_sut.SendResponse(_req));
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Once);

			_req.HttpMethod = "PUT";
			Assert.IsTrue(_sut.SendResponse(_req));
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Exactly(2));

			_req.HttpMethod = "DELETE";
			Assert.IsTrue(_sut.SendResponse(_req));
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Exactly(3));

			_socket.Verify(x => x.Send200_OK(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
			_socket.Verify(x => x.Send404_NotFound(), Times.Never);
		}

		[Test]
		public void Default_endpoint_returns_all_projects()
		{
			_req.HttpMethod = "GET";

			_config.Setup(x => x.GetProjects()).Returns(new[]
				{
					_projectOne, _projectTwo
				});
			string expectedJson = @"[" + _jsonOne + "," + _jsonTwo + "]";
			var expectedBytes = Encoding.UTF8.GetBytes(expectedJson);

			_sut.SendResponse(_req);

			_socket.Verify(x => x.Send200_OK("application/json", expectedBytes.Length), Times.Once());
			_socket.Verify(x => x.Send404_NotFound(), Times.Never);
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);

			_socket.Verify(x => x.Send(expectedBytes, expectedBytes.Length), Times.Once);
		}
	}
}