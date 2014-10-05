namespace Deployer.Services.Api.Interfaces
{
	public interface IApiService
	{
		bool CanRespond(ApiRequest request);
		bool SendResponse(ApiRequest request);
	}
}