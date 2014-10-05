using System;
using NeonMika.Requests;

namespace NeonMika.Responses
{
	public abstract class Responder : IDisposable
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