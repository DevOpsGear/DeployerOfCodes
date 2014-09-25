using System;
using System.Collections;
using System.IO;
using System.Text;
using Json.NETMF;

namespace Deployer.Services.Micro.Web
{
	public class WebUtility : IWebUtility
	{
		private readonly IGarbage _garbage;

		public WebUtility(IGarbage garbage)
		{
			_garbage = garbage;
		}

		public int WriteJsonObject(Stream output, object obj)
		{
			var data = JsonSerializer.SerializeObject(obj);
			var encoded = Encoding.UTF8.GetBytes(data);
			output.Write(encoded, 0, encoded.Length);
			return encoded.Length;
		}

		public void WriteJsonObject(IWebRequest req, object obj)
		{
			var output = req.GetRequestStream();
			req.ContentLength = WriteJsonObject(output, obj);
		}

		public Hashtable ReadJsonObject(IWebRequest req, int bufferSize)
		{
			var response = ReadText(req, bufferSize);
			return JsonSerializer.DeserializeString(response) as Hashtable;
		}

		public string ReadText(IWebRequest req, int bufferSize)
		{
			int read;
			var result = new byte[bufferSize];
			using (var res = req.GetResponse())
			{
				using (var stream = res.GetResponseStream())
				{
					read = stream.Read(result, 0, result.Length);
				}
			}
			_garbage.Collect();
			var chars = Encoding.UTF8.GetChars(result, 0, read);
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
			url = url.Trim() ?? string.Empty;
			if (url.Substring(url.Length - 1) != "/")
				url += "/";
			return url;
		}
	}
}