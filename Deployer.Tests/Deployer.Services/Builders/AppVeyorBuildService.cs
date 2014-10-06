using System;
using System.Collections;
using Deployer.Services.Micro.Web;
using Deployer.Services.Models;
using Json.NETMF;

namespace Deployer.Services.Builders
{
	public class AppVeyorBuildService : IBuildService
	{
		private readonly IWebRequestFactory _webFactory;
		private readonly IWebUtility _netio;
		private string _apiToken;
		private string _accountName;
		private string _projectSlug;
		private string _branch;
		private string _buildVersion;

		public AppVeyorBuildService(IWebRequestFactory webFactory, IWebUtility netio)
		{
			_webFactory = webFactory;
			_netio = netio;
		}

		// TODO: Figure out HTTPS support?
		// https://www.ghielectronics.com/community/forum/topic?id=13927
		// https://www.ghielectronics.com/community/codeshare/entry/614

		// http://www.appveyor.com/docs/api/projects-builds#start-build
		public BuildState StartBuild(Hashtable config)
		{
			try
			{
				DecodeConfig(config);
				_buildVersion = null;

				var req = CreateRequest("builds", "POST");
				var body = new Hashtable
					{
						{"accountName", _accountName},
						{"projectSlug", _projectSlug},
						{"branch", _branch}
					};

				WriteBody(req, body);
				var result = GetValue(req);
				_buildVersion = result["version"].ToString();
				var status = result["status"].ToString();
				return DecodeBuildState(status);
			}
			catch (Exception ex)
			{
				return new BuildState(BuildStatus.Failed);
			}
		}

		public BuildState GetStatus()
		{
			try
			{
				var endPoint = "/projects/" + _accountName + "/" + _projectSlug + "/build/" + _buildVersion;
				var req = CreateRequest(endPoint);
				var result = GetValue(req);
				var build = (Hashtable) result["build"];
				var status = build["status"].ToString();
				return DecodeBuildState(status);
			}
			catch (Exception ex)
			{
				return new BuildState(BuildStatus.Failed);
			}
		}

		private BuildState DecodeBuildState(string status)
		{
			switch (status)
			{
				case "queued":
					return new BuildState(BuildStatus.Queued);
				case "running":
					return new BuildState(BuildStatus.Running);
				case "failed":
					return new BuildState(BuildStatus.Failed);
				default:
					return new BuildState(BuildStatus.Failed);
			}
		}

		// http://www.appveyor.com/docs/api/projects-builds#cancel-build
		public void CancelBuild()
		{
			// DELETE /api/builds/{accountName}/{projectSlug}/{buildVersion}
			var endPoint = "builds/" + _accountName + "/" + _projectSlug + "/" + _buildVersion;
			var req = CreateRequest(endPoint, "DELETE");
			var result = GetValue(req);
		}

		private void DecodeConfig(Hashtable hash)
		{
			if (hash == null)
				return;
			_apiToken = hash["apiToken"] as string;
			_accountName = hash["accountName"] as string;
			_projectSlug = hash["projectSlug"] as string;
			_branch = hash["branch"] as string;
		}

		private IWebRequest CreateRequest(string apiEndpoint, string method = "GET")
		{
			var req = _webFactory.CreateRequest("http://ci.appveyor.com/api/", apiEndpoint, method);
			req.ContentType = "application/json";
			req.AddHeader("Authorization", "Bearer " + _apiToken);
			return req;
		}

		private void WriteBody(IWebRequest req, object body)
		{
			_netio.WriteJsonObject(req, body);
		}

		private Hashtable GetValue(IWebRequest req)
		{
			return _netio.ReadJsonObject(req, 3072);
		}
	}
}