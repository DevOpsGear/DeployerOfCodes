using System;
using System.Collections;
using System.Text;
using Deployer.Services.Api.Interfaces;
using Deployer.Services.Builders;
using Deployer.Services.Config;
using Deployer.Services.Config.Interfaces;
// ReSharper disable RedundantUsingDirective
using Deployer.Services.Models;
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
			try
			{
				var url = new UrlSplitter(request.Url);
				HandleStuff(request, url);
			}
			catch(Exception)
			{
				request.Client.Send500_Failure();
			}
			return true;
		}

		private void HandleStuff(ApiRequest request, UrlSplitter url)
		{
			if(url.Endpoint == "projects")
			{
				if(url.Id == "")
				{
					HandleDefault(request);
					return;
				}

				if(url.Option == "")
				{
					HandleProject(url.Id, request);
					return;
				}

				if(url.Option == "build" && url.Moar == "")
				{
					HandleBuild(url.Id, request);
					return;
				}
			}
			request.Client.Send404_NotFound();
		}

		private void HandleDefault(ApiRequest request)
		{
			if(request.HttpMethod == "GET")
			{
				var projects = _configurationService.GetProjects();
				var hash = ConfigHashifier.Hashify(projects);
				var json = JsonSerializer.SerializeObject(hash);
				var bytes = Encoding.UTF8.GetBytes(json);
				request.Client.Send200_OK("application/json", bytes.Length);
				request.Client.Send(bytes, bytes.Length);
				return;
			}

			if(request.HttpMethod == "PUT")
			{
				var proj = new ProjectModel();
				SnarfProject(request, proj);
				proj.Slug = "";
				_configurationService.SaveProject(proj);
				request.Client.Send200_OK("application/json");
				return;
			}

			request.Client.Send405_MethodNotAllowed();
		}

		private void HandleProject(string slug, ApiRequest request)
		{
			if(request.HttpMethod == "GET")
			{
				GetOneProject(slug, request);
				return;
			}

			if(request.HttpMethod == "PUT")
			{
				PutOneProject(slug, request);
				return;
			}

			if(request.HttpMethod == "DELETE")
			{
				DeleteOneProject(slug, request);
				return;
			}

			request.Client.Send405_MethodNotAllowed();
		}

		private void GetOneProject(string slug, ApiRequest request)
		{
			try
			{
				var proj = _configurationService.GetProject(slug);
				var hash = ConfigHashifier.Hashify(proj);
				var json = JsonSerializer.SerializeObject(hash);
				var bytes = Encoding.UTF8.GetBytes(json);
				request.Client.Send200_OK("application/json", bytes.Length);
				request.Client.Send(bytes, bytes.Length);
			}
			catch(ProjectDoesNotExistException)
			{
				request.Client.Send404_NotFound();
			}
		}

		private void PutOneProject(string slug, ApiRequest request)
		{
			try
			{
				var proj = _configurationService.GetProject(slug);
				SnarfProject(request, proj);
				_configurationService.SaveProject(proj);
				request.Client.Send200_OK("application/json");
			}
			catch(ProjectDoesNotExistException)
			{
				request.Client.Send404_NotFound();
			}
			catch(Exception)
			{
				request.Client.Send400_BadRequest();
			}
		}

		private static void SnarfProject(ApiRequest request, ProjectModel proj)
		{
			var buffer = new byte[1024];
			var countBytes = request.Body.ReadBytes(buffer);
			var chars = Encoding.UTF8.GetChars(buffer, 0, countBytes);
			var json = new string(chars);
			var project = JsonSerializer.DeserializeString(json) as Hashtable;

			proj.Title = project["title"] as string;
			proj.Subtitle = project["subtitle"] as string;
			proj.Rank = Int32.Parse(project["rank"].ToString());
			proj.Provider = (BuildServiceProvider) Int32.Parse(project["provider"].ToString());
		}

		private void DeleteOneProject(string slug, ApiRequest request)
		{
			try
			{
				_configurationService.DeleteProject(slug);
				request.Client.Send200_OK("application/json");
			}
			catch(ProjectDoesNotExistException)
			{
				request.Client.Send404_NotFound();
			}
			catch(Exception)
			{
				request.Client.Send400_BadRequest();
			}
		}

		private void HandleBuild(string slug, ApiRequest request)
		{
			throw new System.NotImplementedException();
		}
	}
}