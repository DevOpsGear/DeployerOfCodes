using Deployer.Services.Micro;
using System.IO;

namespace Deployer.Tests.SpiesFakes
{
	public class WebResponseSpy : IHttpWebResponse
	{
		private byte[] _data;

		public WebResponseSpy()
		{
			_data = new byte[] {};
		}

		public void SetData(byte[] data)
		{
			_data = data;
		}

		public Stream GetResponseStream()
		{
			return new MemoryStream(_data);
		}

		public void Dispose()
		{
		}
	}
}