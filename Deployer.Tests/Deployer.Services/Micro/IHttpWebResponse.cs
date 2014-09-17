using System;
using System.IO;

namespace Deployer.Services.Micro
{
	public interface IHttpWebResponse : IDisposable
	{
		Stream GetResponseStream();
	}
}
