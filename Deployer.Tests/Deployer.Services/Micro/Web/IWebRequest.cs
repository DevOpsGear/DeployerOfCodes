﻿using System.IO;

namespace Deployer.Services.Micro.Web
{
	public interface IWebRequest
	{
		string ContentType { get; set; }
		long ContentLength { get; set; }
		void AddHeader(string key, string value);
		IHttpWebResponse GetResponse();
		Stream GetRequestStream();
	}
}