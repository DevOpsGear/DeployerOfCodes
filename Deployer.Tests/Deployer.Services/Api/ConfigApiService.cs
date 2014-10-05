using Deployer.Services.Api.Interfaces;
using Deployer.Services.Config;
// ReSharper disable RedundantUsingDirective
using Deployer.Services.Util;

// ReSharper restore RedundantUsingDirective

namespace Deployer.Services.Api
{
	public class ConfigApiService : IApiService
	{
		private readonly IConfigurationService _configurationService;

		public ConfigApiService(IConfigurationService configurationService)
		{
			_configurationService = configurationService;
		}

		public bool CanRespond(ApiRequest request)
		{
			return request.Url.StartsWith("config");
		}

		public bool SendResponse(ApiRequest request)
		{
			return false;
		}
	}
}