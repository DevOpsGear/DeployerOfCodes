using Deployer.Services.Api.Interfaces;
using Deployer.Services.Config.Interfaces;
using NeonMika.Interfaces;
using NeonMika.Util;

namespace Deployer.Services.Api
{
	public class AuthApiService : IApiService
	{
		private readonly IConfigurationService _configurationService;
        private readonly IGarbage _garbage;

        public AuthApiService(IConfigurationService configurationService, IGarbage garbage)
		{
			_configurationService = configurationService;
			_garbage = garbage;
		}

		public bool CanRespond(ApiRequest request)
		{
			return request.Url.StartsWith("auth");
		}

		public bool SendResponse(ApiRequest request)
		{
			return false;
		}
	}
}