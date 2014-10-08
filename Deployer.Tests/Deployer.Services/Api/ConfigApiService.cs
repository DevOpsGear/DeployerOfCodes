using System.Text;
using Deployer.Services.Api.Interfaces;
using Deployer.Services.Config.Interfaces;
// ReSharper disable RedundantUsingDirective
using Deployer.Services.Util;
using Json.NETMF;

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
			return request.Url.StartsWith("projects");
		}

		public bool SendResponse(ApiRequest request)
		{
			var segments = request.Url.EasySplit("/");
			if(segments[0] == "projects")
			{
				var countArgs = segments.Length - 1;
				if(segments[segments.Length - 1] == string.Empty)
					countArgs--;
				switch(countArgs)
				{
					case 0:
						GetList(request);
						break;

					case 1:
						HandleProject(segments[1], request);
						break;

					case 2:
						if(segments[2] == "build")
							HandleBuild(segments[1], request);
						else
							request.Client.Send404_NotFound();
						break;

					default:
						request.Client.Send404_NotFound();
						break;
				}
			}
			else
			{
				request.Client.Send404_NotFound();
			}
			return true;
		}

		private void GetList(ApiRequest request)
		{
			if(request.HttpMethod == "GET")
			{
				var all = _configurationService.GetProjects();
				var json = JsonSerializer.SerializeObject(all);
				var bytes = Encoding.UTF8.GetBytes(json);
				request.Client.Send200_OK("application/json", bytes.Length);
				request.Client.Send(bytes, bytes.Length);
			}
			else
			{
				request.Client.Send405_MethodNotAllowed();
			}
		}

		private void HandleProject(string slug, ApiRequest request)
		{
			_configurationService.GetProject(slug);
			if(request.HttpMethod == "GET")
			{
			}
		}

		private void HandleBuild(string slug, ApiRequest request)
		{
			throw new System.NotImplementedException();
		}
	}
}