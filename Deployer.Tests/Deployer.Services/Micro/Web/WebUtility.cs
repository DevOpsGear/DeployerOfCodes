using System;
using System.Collections;
using System.Text;
using Json.NETMF;
using NeonMika.Interfaces;

namespace Deployer.Services.Micro.Web
{
	public class WebUtility : IWebUtility
	{
		private readonly IGarbage _garbage;

        public WebUtility(IGarbage garbage)
		{
			_garbage = garbage;
		}

		public void WriteJsonObject(IWebRequest req, object obj)
		{
			var data = JsonSerializer.SerializeObject(obj);
			var encoded = Encoding.UTF8.GetBytes(data);
			req.ContentLength = encoded.Length;
			var output = req.GetRequestStream();
			output.Write(encoded, 0, encoded.Length);

			// ReSharper disable RedundantAssignment
			// Clean up
			data = null;
			encoded = null;
			_garbage.Collect();
			// ReSharper restore RedundantAssignment
		}

		public Hashtable ReadJsonObject(IWebRequest req, int bufferSize)
		{
			_garbage.Collect();
			var response = ReadText(req, bufferSize);
			return JsonSerializer.DeserializeString(response) as Hashtable;
		}

		public string ReadText(IWebRequest req, int bufferSize)
		{
			int read;
			var result = new byte[bufferSize];
			using(var res = req.GetResponse())
			{
				using(var stream = res.GetResponseStream())
				{
					read = stream.Read(result, 0, result.Length);
				}
			}
			_garbage.Collect();
			var chars = Encoding.UTF8.GetChars(result, 0, read);
			result = null;
			_garbage.Collect();
			return new string(chars);
		}

		public string GetHttpBasicAuthToken(string username, string password)
		{
			var unpw = username + ":" + password;
			var bytes = Encoding.UTF8.GetBytes(unpw);
			return Convert.ToBase64String(bytes);
		}

		public string NormalizeUrl(string url)
		{
			url = url.Trim();
			if(url.Substring(url.Length - 1) != "/")
				url += "/";
			return url;
		}
	}
}