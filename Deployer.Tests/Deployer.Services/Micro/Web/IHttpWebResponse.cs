using System;
using System.IO;

namespace Deployer.Services.Micro.Web
{
	public interface IHttpWebResponse : IDisposable
	{
		Stream GetResponseStream();
	}
}
