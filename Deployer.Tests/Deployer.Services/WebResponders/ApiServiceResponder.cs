using Deployer.Services.Api;
using Deployer.Services.Api.Interfaces;
using NeonMika.Requests;
using NeonMika.Responses;

namespace Deployer.Services.WebResponders
{
	public class ApiServiceResponder : Responder
	{
		private readonly IApiService _apiService;

		public ApiServiceResponder(IApiService apiService)
		{
			_apiService = apiService;
		}

		public override bool CanRespond(Request e)
		{
			var request = new ApiRequest
				{
					Client = new ApiSocketWrapper(e.Client),
					HttpMethod = e.HttpMethod,
					Url = e.Url,
					Headers = e.Headers,
					GetArguments = e.GetArguments,
					Body = new ApiReadBodyWrapper(e.Body)
				};
			return _apiService.CanRespond(request);
		}

		public override bool SendResponse(Request e)
		{
			var request = new ApiRequest
				{
					Client = new ApiSocketWrapper(e.Client),
					HttpMethod = e.HttpMethod,
					Url = e.Url,
					Headers = e.Headers,
					GetArguments = e.GetArguments,
					Body = new ApiReadBodyWrapper(e.Body)
				};
			return _apiService.SendResponse(request);
		}
	}
}