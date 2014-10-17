using Deployer.Services.Api;
using Deployer.Services.Api.Interfaces;
using Deployer.Services.Builders;
using Deployer.Services.Config;
using Deployer.Services.Config.Interfaces;
using Deployer.Services.Micro;
using Deployer.Services.Models;
using Json.NETMF;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Net;
using System.Text;

namespace Deployer.Tests.Api
{
	[TestFixture]
	internal class ConfigApiServiceTests
	{
		private FakeApiReadBody _readBody;
		private Mock<IApiSocket> _socket;
		private Mock<IConfigurationService> _config;
		private Mock<IGarbage> _garbage;
		private ConfigApiService _sut;
		private ApiRequest _req;
		private ProjectModel _projectOne;
		private ProjectModel _projectTwo;
		private string _jsonOne;

		[SetUp]
		public void BeforeEachTest()
		{
			_projectOne = new ProjectModel("proj-1", "Project One", "Subtitle One", 1, BuildServiceProvider.AppVeyor);
			_projectTwo = new ProjectModel("proj-2", "Project Two", "Subtitle Two", 2, BuildServiceProvider.TeamCity);
			_jsonOne = JsonSerializer.SerializeObject(ConfigHashifier.Hashify(_projectOne));

			_readBody = new FakeApiReadBody();
			_socket = new Mock<IApiSocket>();
			_config = new Mock<IConfigurationService>();
			_garbage = new Mock<IGarbage>();

			_req = new ApiRequest
				{
					Headers = new Hashtable(),
					GetArguments = new Hashtable(),
					Body = _readBody,
					HttpMethod = "GET",
					Url = "projects/",
					Client = _socket.Object,
				};
			_sut = new ConfigApiService(_config.Object, _garbage.Object);
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
		public void Rejects_unsupported_endpoints_with_404()
		{
			_req.HttpMethod = "GET";
			_req.Url = "blerg/";

			SutSendResponse();

			_socket.Verify(x => x.Send404_NotFound(), Times.Once);
		}

		[Test]
		public void Default_endpoint_rejects_delete_with_405()
		{
			_req.HttpMethod = "DELETE";
			_req.Url = "projects/";

			SutSendResponse();

			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Once);
		}

		[Test]
		public void Default_endpoint_rejects_post_with_405()
		{
			_req.HttpMethod = "POST";
			_req.Url = "projects/";

			SutSendResponse();

			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Once);
		}

		[Test]
		public void One_project_endpoint_rejects_post_with_405()
		{
			_req.HttpMethod = "POST";
			_req.Url = "projects/bad-slug";

			SutSendResponse();

			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Once);
		}

		[Test]
		public void Returns_500_for_unexpected_exception()
		{
			_req.HttpMethod = "GET";
			_config.Setup(x => x.GetProjects()).Throws(new ProtocolViolationException());

			SutSendResponse();

			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Once);
		}

		[Test]
		public void Returns_404_if_too_many_params()
		{
			_req.HttpMethod = "GET";
			_req.Url = "projects/slug/dingus/flark";

			SutSendResponse();

			_socket.Verify(x => x.Send404_NotFound(), Times.Once);
		}

		[Test]
		public void Default_endpoint_supports_get()
		{
			_req.HttpMethod = "GET";
			_req.Url = "projects/";

			SutSendResponse();

			_socket.Verify(x => x.Send200_OK("application/json", It.IsAny<int>()), Times.Once);
		}

		[Test]
		public void Default_endpoint_supports_put()
		{
			_req.HttpMethod = "PUT";
			_req.Url = "projects/";
			var bytes = Encoding.UTF8.GetBytes(_jsonOne);

			_readBody.SetUp(bytes);

			_config.Setup(x => x.SaveProject(It.IsAny<ProjectModel>()))
			       .Callback((ProjectModel proj) =>
				       {
					       Assert.AreEqual("", proj.Slug);
					       Assert.AreEqual(_projectOne.Title, proj.Title);
					       Assert.AreEqual(_projectOne.Subtitle, proj.Subtitle);
					       Assert.AreEqual(_projectOne.Rank, proj.Rank);
					       Assert.AreEqual(_projectOne.Provider, proj.Provider);
				       });

			SutSendResponse();

			_socket.Verify(x => x.Send200_OK("application/json"), Times.Once);
			_config.Verify(x => x.SaveProject(It.IsAny<ProjectModel>()), Times.Once);

			_socket.Verify(x => x.Send404_NotFound(), Times.Never);
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void Default_endpoint_rejects_other_verbs()
		{
			_req.HttpMethod = "POST";
			SutSendResponse();
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Once);

			_req.HttpMethod = "DELETE";
			SutSendResponse();
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Exactly(2));

			_socket.Verify(x => x.Send200_OK(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
			_socket.Verify(x => x.Send404_NotFound(), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void Default_endpoint_returns_all_projects()
		{
			var projects = new[]
				{
					_projectOne,
					_projectTwo
				};
			_req.HttpMethod = "GET";
			_config.Setup(x => x.GetProjects()).Returns(projects);
			string expectedJson = JsonSerializer.SerializeObject(ConfigHashifier.Hashify(projects));
			var expectedBytes = Encoding.UTF8.GetBytes(expectedJson);

			SutSendResponse();

			_socket.Verify(x => x.Send200_OK("application/json", expectedBytes.Length), Times.Once);
			_socket.Verify(x => x.Send(expectedBytes, expectedBytes.Length), Times.Once);

			_socket.Verify(x => x.Send404_NotFound(), Times.Never);
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void One_project_endpoint_returns_correct_json()
		{
			_req.HttpMethod = "GET";
			_req.Url = "projects/" + _projectOne.Slug;
			_config.Setup(x => x.GetProject(_projectOne.Slug)).Returns(_projectOne);
			string expectedJson = _jsonOne;
			var expectedBytes = Encoding.UTF8.GetBytes(expectedJson);

			SutSendResponse();

			_socket.Verify(x => x.Send200_OK("application/json", expectedBytes.Length), Times.Once);
			_socket.Verify(x => x.Send(expectedBytes, expectedBytes.Length), Times.Once);

			_socket.Verify(x => x.Send404_NotFound(), Times.Never);
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void One_project_endpoint_returns_404_if_bad_slug()
		{
			_req.HttpMethod = "GET";
			_req.Url = "projects/bad-slug";
			_config.Setup(x => x.GetProject("bad-slug")).Throws(new ProjectDoesNotExistException("bad-slug"));

			SutSendResponse();

			_socket.Verify(x => x.Send404_NotFound(), Times.Once);

			_socket.Verify(x => x.Send200_OK("application/json", It.IsAny<int>()), Times.Never);
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
			_socket.Verify(x => x.Send(It.IsAny<byte[]>(), It.IsAny<int>()), Times.Never);
		}

		[Test]
		public void One_project_endpoint_saves_on_put()
		{
			var modifiedProject = new ProjectModel
				{
					Title = "Modified title",
					Subtitle = "Modified subtitle",
					Rank = 55,
					Provider = BuildServiceProvider.TeamCity
				};
			var hash = ConfigHashifier.Hashify(modifiedProject);
			var json = JsonSerializer.SerializeObject(hash);
			var bytesModifiedProject = Encoding.UTF8.GetBytes(json);

			_config.Setup(x => x.GetProject(_projectOne.Slug)).Returns(_projectOne);
			_config.Setup(x => x.SaveProject(It.IsAny<ProjectModel>()))
			       .Callback((ProjectModel proj) =>
				       {
					       Assert.AreEqual(_projectOne.Slug, proj.Slug);
					       Assert.AreEqual(modifiedProject.Title, proj.Title);
					       Assert.AreEqual(modifiedProject.Subtitle, proj.Subtitle);
					       Assert.AreEqual(modifiedProject.Rank, proj.Rank);
					       Assert.AreEqual(modifiedProject.Provider, proj.Provider);
				       });

			_req.HttpMethod = "PUT";
			_req.Url = "projects/" + _projectOne.Slug;
			_readBody.SetUp(bytesModifiedProject);

			SutSendResponse();

			_config.Verify(x => x.SaveProject(It.IsAny<ProjectModel>()), Times.Once);
			_socket.Verify(x => x.Send200_OK("application/json"), Times.Once);

			_socket.Verify(x => x.Send404_NotFound(), Times.Never);
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void One_project_endpoint_put_bad_slug_returns_404()
		{
			_config.Setup(x => x.GetProject(It.IsAny<string>())).Throws(new ProjectDoesNotExistException("whatever"));

			_req.HttpMethod = "PUT";
			_req.Url = "projects/whatever";

			SutSendResponse();

			_socket.Verify(x => x.Send404_NotFound(), Times.Once);

			_socket.Verify(x => x.Send200_OK(It.IsAny<string>()), Times.Never);
			_socket.Verify(x => x.Send400_BadRequest(), Times.Never);
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void One_project_endpoint_put_other_error_returns_400()
		{
			_config.Setup(x => x.GetProject(It.IsAny<string>())).Throws(new Exception("something-else-happened"));

			_req.HttpMethod = "PUT";
			_req.Url = "projects/whatever";

			SutSendResponse();

			_socket.Verify(x => x.Send400_BadRequest(), Times.Once);

			_socket.Verify(x => x.Send200_OK(It.IsAny<string>()), Times.Never);
			_socket.Verify(x => x.Send404_NotFound(), Times.Never);
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void One_project_endpoint_delete_existing_project()
		{
			_req.HttpMethod = "DELETE";
			_req.Url = "projects/slug-gone";

			SutSendResponse();

			_config.Verify(x => x.DeleteProject("slug-gone"), Times.Once);
			_socket.Verify(x => x.Send200_OK("application/json"), Times.Once);

			_socket.Verify(x => x.Send404_NotFound(), Times.Never);
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void One_project_endpoint_delete_bad_slug_returns_404()
		{
			_req.HttpMethod = "DELETE";
			_req.Url = "projects/slug-bad";
			_config.Setup(x => x.DeleteProject("slug-bad")).Throws(new ProjectDoesNotExistException("slug-bad"));

			SutSendResponse();

			_config.Verify(x => x.DeleteProject("slug-bad"), Times.Once);
			_socket.Verify(x => x.Send404_NotFound(), Times.Once);

			_socket.Verify(x => x.Send200_OK("application/json"), Times.Never);
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void One_project_endpoint_delete_error_400()
		{
			_req.HttpMethod = "DELETE";
			_req.Url = "projects/slug-bad";
			_config.Setup(x => x.DeleteProject("slug-bad")).Throws(new Exception("something-else"));

			SutSendResponse();

			_config.Verify(x => x.DeleteProject("slug-bad"), Times.Once);
			_socket.Verify(x => x.Send400_BadRequest(), Times.Once);

			_socket.Verify(x => x.Send200_OK("application/json"), Times.Never);
			_socket.Verify(x => x.Send404_NotFound(), Times.Never);
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void Build_endpoint_rejects_post_with_405()
		{
			_req.HttpMethod = "POST";
			_req.Url = "projects/slug/build";

			SutSendResponse();

			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Once);

			_socket.Verify(x => x.Send404_NotFound(), Times.Never);
			_socket.Verify(x => x.Send200_OK("application/json"), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void Build_endpoint_rejects_delete_with_405()
		{
			_req.HttpMethod = "DELETE";
			_req.Url = "projects/slug/build";

			SutSendResponse();

			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Once);

			_socket.Verify(x => x.Send404_NotFound(), Times.Never);
			_socket.Verify(x => x.Send200_OK("application/json"), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void Build_endpoint_accepts_get()
		{
			var hash = new Hashtable
				{
					{"key1", "value1"}
				};
			var json = JsonSerializer.SerializeObject(hash);
			var expectedBytes = Encoding.UTF8.GetBytes(json);
			_req.HttpMethod = "GET";
			_req.Url = "projects/slug/build";
			_config.Setup(x => x.GetBuildParams("slug")).Returns(hash);

			SutSendResponse();

			_config.Verify(x => x.GetBuildParams("slug"), Times.Once);
			_socket.Verify(x => x.Send200_OK("application/json", expectedBytes.Length), Times.Once);
			_socket.Verify(x => x.Send(expectedBytes, expectedBytes.Length), Times.Once);

			_socket.Verify(x => x.Send404_NotFound(), Times.Never);
			_socket.Verify(x => x.Send405_MethodNotAllowed(), Times.Never);
			_socket.Verify(x => x.Send500_Failure(It.IsAny<string>()), Times.Never);
		}

		[Test]
		public void Build_endpoint_accepts_put()
		{
			var hash = new Hashtable
				{
					{"key1", "value1"}
				};
			var json = JsonSerializer.SerializeObject(hash);
			var bytesBuild = Encoding.UTF8.GetBytes(json);
			_req.HttpMethod = "PUT";
			_req.Url = "projects/slug/build";
			_readBody.SetUp(bytesBuild);

			SutSendResponse();

			_config.Verify(x => x.SaveBuildParams("slug", hash), Times.Once);
			_socket.Verify(x => x.Send200_OK("application/json"), Times.Once);
		}

		private void SutSendResponse()
		{
			Assert.IsTrue(_sut.SendResponse(_req));
		}
	}
}