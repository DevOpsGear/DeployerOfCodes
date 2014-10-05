using Deployer.Services.Api.Interfaces;
// ReSharper disable RedundantUsingDirective
using Deployer.Services.Util;

// ReSharper restore RedundantUsingDirective

namespace Deployer.Services.Api
{
	public class AuthApiService : IApiService
	{
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