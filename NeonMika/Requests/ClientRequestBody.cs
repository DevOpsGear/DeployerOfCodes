using System;
using System.Net.Sockets;
using System.Threading;

namespace NeonMika.Requests
{
	public class ClientRequestBody
	{
		private readonly byte[] _preRead;
		private readonly Socket _client;
		private int _preIndex;

		public ClientRequestBody(byte[] preRead, Socket client)
		{
			_preRead = preRead;
			_client = client;
			_preIndex = 0;
		}

		public int ReadBytes(byte[] buffer)
		{
			// Anything remaining in pre-read buffer?
			var remainingInPreBuffer = _preRead.Length - _preIndex;
			if (remainingInPreBuffer > 0)
			{
				var countBytesToCopy = Math.Min(remainingInPreBuffer, buffer.Length);
				Array.Copy(_preRead, _preIndex, buffer, 0, countBytesToCopy);
				_preIndex += countBytesToCopy;
				return countBytesToCopy;
			}

			Thread.Sleep(Settings.SLEEP_WAIT_FOR_SOCKET_DATA);
			var availableBytes = _client.Available;
			if (availableBytes > 0)
			{
				var max = Math.Min(availableBytes, buffer.Length);
				return _client.Receive(buffer, max, SocketFlags.None);
			}
			return 0;
		}
	}
}