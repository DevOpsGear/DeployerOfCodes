using System;
using System.Collections;
using Deployer.Services.Micro.Web;
using Deployer.Services.Models;
using Json.NETMF;

namespace Deployer.Services.Builders
{
	public class TeamCityBuildService : IBuildService
	{
		private readonly IWebRequestFactory _webFactory;
		private readonly IWebUtility _webio;
		private string _apiRoot;
		private string _buildId;
		private string _username;
		private string _password;

		public TeamCityBuildService(IWebRequestFactory webFactory, IWebUtility webio)
		{
			_webFactory = webFactory;
			_webio = webio;
		}

		// http://confluence.jetbrains.com/display/TCD8/REST+API
		// http://confluence.jetbrains.com/display/TCD8/REST+API#RESTAPI-TriggeringaBuild
		// TODO: JSON
		public BuildState StartBuild(Hashtable config)
		{
			try
			{
				DecodeConfig(config);

				var req = CreateRequest(_apiRoot, "buildQueue", "POST");
				var body = @"<build><buildType id=""" + _buildId + @""" /></build>";
				_webio.WriteJsonObject(req, body);
				var todo = _webio.ReadJsonObject(req, 4096);
				return new BuildState(BuildStatus.Queued);
			}
			catch(Exception ex)
			{
				return new BuildState(BuildStatus.Failed);
			}
		}

		public BuildState GetStatus()
		{
			return new BuildState(BuildStatus.Running);
		}

		public void CancelBuild()
		{
		}

		private void DecodeConfig(Hashtable hash)
		{
			if(hash == null)
				throw new Exception("bad config");
			var url = _webio.NormalizeUrl(hash["url"] as string);
			_apiRoot = url + "httpAuth/app/rest/";
			_buildId = hash["buildId"] as string;
			_username = hash["username"] as string;
			_password = hash["password"] as string;
		}

		private IWebRequest CreateRequest(string apiRoot, string apiEndpoint, string method = "GET")
		{
			var req = _webFactory.CreateRequest(apiRoot, apiEndpoint, method);
			req.ContentType = "application/xml";
			req.AddHeader("Accept", "application/json");
			req.AddHeader("Authorization", "Basic " + _webio.GetHttpBasicAuthToken(_username, _password));
			return req;
		}
	}
}