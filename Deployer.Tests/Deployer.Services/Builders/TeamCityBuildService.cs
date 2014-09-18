using System;
using System.Collections;
using System.Text;
using System.Threading;
using Deployer.Services.Micro;
using Deployer.Services.Models;
using Json.NETMF;

namespace Deployer.Services.Builders
{
	public class TeamCityBuildService : IBuildService
	{
		private readonly IWebRequestFactory _webFactory;
		private readonly IGarbage _garbage;
		private string _apiRoot;
		private string _buildId;

		public TeamCityBuildService(IWebRequestFactory webFactory, IGarbage garbage)
		{
			_webFactory = webFactory;
			_garbage = garbage;
		}

		// http://confluence.jetbrains.com/display/TCD8/REST+API#RESTAPI-TriggeringaBuild
		public BuildState StartBuild(string config)
		{
			try
			{
				DecodeConfig(config);

				var req = CreateRequest(_apiRoot, "buildQueue", "POST");
				var body = @"<build><buildType id=""" + _buildId + @""" /></build>";
				WriteBody(req, body);
				var result = GetValue(req);
				return new BuildState(BuildStatus.Queued);
			}
			catch (Exception ex)
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

		private void DecodeConfig(string config)
		{
			var hash = JsonSerializer.DeserializeString(config) as Hashtable;
			if (hash == null) return;
			var url = hash["url"] as string;
			if (!url.EndsWith("/"))
				url += "/";
			_apiRoot = url + "httpAuth/app/rest/";
			_buildId = hash["buildId"] as string;
		}

		private IWebRequest CreateRequest(string apiRoot, string apiEndpoint, string method = "GET")
		{
			var req = _webFactory.CreateRequest(apiRoot, apiEndpoint, method);
			req.ContentType = "application/json";
			req.AddHeader("Authorization", "Bearer TOKEN");
			return req;
		}

		private void WriteBody(IWebRequest req, string body)
		{
			var encodedBody = Encoding.UTF8.GetBytes(body);
			req.ContentLength = encodedBody.Length;
			var bodyStream = req.GetRequestStream();
			bodyStream.Write(encodedBody, 0, encodedBody.Length);
		}

		private Hashtable GetValue(IWebRequest req)
		{
			int read;
			_garbage.Collect();
			var result = new byte[8192]; // TODO: Too big?
			using (var res = req.GetResponse())
			{
				using (var stream = res.GetResponseStream())
				{
					do
					{
						read = stream.Read(result, 0, result.Length);
						Thread.Sleep(20);
					} while (read != 0);
				}
			}
			var response = Encoding.UTF8.GetChars(result, 0, read).ToString();
			var val = JsonSerializer.DeserializeString(response) as Hashtable;
			_garbage.Collect();
			return val;
		}
	}
}