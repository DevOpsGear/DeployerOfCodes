using Deployer.Services.Api.Interfaces;
using System;

namespace Deployer.Services.Api
{
	public class ShortBodyReader
	{
		public static int ReadBody(IApiReadBody readBody, byte[] buffer)
		{
			int size = 0;
			var internalBuffer = new byte[256];
			while(true)
			{
				var count = readBody.ReadBytes(internalBuffer);
				if(count == 0)
					break;
				Array.Copy(internalBuffer, 0, buffer, size, count);
				size += count;
			}
			return size;
		}
	}
}