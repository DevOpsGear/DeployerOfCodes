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
		private const int BufferSize = 4096;

		public TeamCityBuildService(IWebRequestFactory webFactory, IWebUtility webio)
		{
			_webFactory = webFactory;
			_webio = webio;
		}

		// http://confluence.jetbrains.com/display/TCD8/REST+API
		// http://confluence.jetbrains.com/display/TCD8/REST+API#RESTAPI-TriggeringaBuild
		public BuildState StartBuild(Hashtable config)
		{
			try
			{
				DecodeConfig(config);

				var req = CreateRequest(_apiRoot, "buildQueue", "POST");
				var body = @"<build><buildType id=""" + _buildId + @""" /></build>";
				_webio.WriteJsonObject(req, body);
				var result = _webio.ReadJsonObject(req, BufferSize);
				return new BuildState(BuildStatus.Queued);
			}
			catch(Exception ex)
			{
				return new BuildState(BuildStatus.Failed);
			}
		}

		public BuildState GetStatus()
		{
			var reqQ = CreateRequest(_apiRoot, "buildQueue/?locator=buildType:" + _buildId);
			var hashQ = _webio.ReadJsonObject(reqQ, BufferSize);
			var countQ = Int32.Parse(hashQ["count"].ToString());
			if (countQ > 0)
			{
				return new BuildState(BuildStatus.Queued);
			}

			// Assume it's the first one
			var reqBuilding = CreateRequest(_apiRoot, "builds/?locator=running:any,count:1,buildType:" + _buildId);
			var hashBuilding = _webio.ReadJsonObject(reqBuilding, BufferSize);
			var countBuilding = Int32.Parse(hashBuilding["count"].ToString());
			if(countBuilding > 0)
			{
				var builds = hashBuilding["build"] as ArrayList;
				var build = builds[0] as Hashtable;
				var tcStatus = build["status"] as string;
				var tcState = build["state"] as string;
				return DecodeState(tcStatus, tcState);
			}
			return new BuildState(BuildStatus.Queued);
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

		private BuildState DecodeState(string tcStatus, string tcState)
		{
			BuildStatus bs;
			switch(tcState.ToLower())
			{
				case "running":
					bs = BuildStatus.Running;
					break;
				case "finished":
					switch(tcStatus.ToLower())
					{
						case "success":
							bs = BuildStatus.Succeeded;
							break;
						default: // failure
							bs = BuildStatus.Failed;
							break;
					}
					break;
				default:
					bs = BuildStatus.Failed;
					break;
			}
			return new BuildState(bs);
		}

		private IWebRequest CreateRequest(string apiRoot, string apiEndpoint, string method = "GET")
		{
			var req = _webFactory.CreateRequest(apiRoot, apiEndpoint, method);
			req.ContentType = "application/xml";
			req.Accept = "application/json";
			req.AddHeader("Authorization", "Basic " + _webio.GetHttpBasicAuthToken(_username, _password));
			return req;
		}
	}
}