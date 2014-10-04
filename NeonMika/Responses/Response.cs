using System;
using System.Text;
using System.Net.Sockets;
using Microsoft.SPOT;
using NeonMika.Requests;

namespace NeonMika.Responses
{
	public abstract class Response : IDisposable
	{
		
		public abstract bool CanRespond(Request e);
		public abstract bool SendResponse(Request e);

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}