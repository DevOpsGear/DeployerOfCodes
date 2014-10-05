using Deployer.Services.Api.Interfaces;
using NeonMika.Requests;

namespace Deployer.App.WebResponders
{
	public class ApiReadBodyWrapper : IApiReadBody
	{
		private readonly ClientRequestBody _body;

		public ApiReadBodyWrapper(ClientRequestBody body)
		{
			_body = body;
		}

		public int ReadBytes(byte[] buffer)
		{
			return _body.ReadBytes(buffer);
		}
	}
}