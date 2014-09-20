using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deployer.Services.Micro;

namespace Deployer.Tests.SpiesFakes
{
	public class WebResponseSpy : IHttpWebResponse
	{
		public Stream GetResponseStream()
		{
			return null;
		}

		public void Dispose()
		{
		}
	}
}