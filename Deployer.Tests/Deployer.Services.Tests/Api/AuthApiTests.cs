using Deployer.Services.Api;
using Deployer.Services.Api.Interfaces;
using Deployer.Services.Config.Interfaces;
using Deployer.Services.Micro;
using Moq;
using NUnit.Framework;
using System.Collections;

namespace Deployer.Tests.Api
{
	[TestFixture]
	internal class AuthApiServiceTests
	{
		private Mock<IApiReadBody> _readBody;
		private Mock<IApiSocket> _socket;
		private Mock<IConfigurationService> _config;
		private Mock<IGarbage> _garbage;
		private AuthApiService _sut;
		private ApiRequest _req;

		[SetUp]
		public void BeforeEachTest()
		{
			_readBody = new Mock<IApiReadBody>();
			_socket = new Mock<IApiSocket>();
			_config = new Mock<IConfigurationService>();
			_garbage = new Mock<IGarbage>();

			_req = new ApiRequest
				{
					Headers = new Hashtable(),
					GetArguments = new Hashtable(),
					Body = _readBody.Object,
					HttpMethod = "GET",
					Url = "auth/",
					Client = _socket.Object,
				};
			_sut = new AuthApiService(_config.Object, _garbage.Object);
		}

		[Test]
		public void Just_cover_it()
		{
			_sut.CanRespond(_req);
			_sut.SendResponse(_req);
		}
	}
}