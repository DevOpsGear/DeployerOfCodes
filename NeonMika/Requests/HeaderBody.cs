using System;

namespace NeonMika.Requests
{
	public class HeaderBody
	{
		private readonly byte[] _header;
		private readonly byte[] _body;

		public HeaderBody(byte[] buffer)
		{
			for (var headerend = 0; headerend < buffer.Length - 3; headerend++)
			{
				if (buffer[headerend] != '\r'
				    || buffer[headerend + 1] != '\n'
				    || buffer[headerend + 2] != '\r'
				    || buffer[headerend + 3] != '\n')
					continue;

				var headerLength = headerend + 4;
				var bodyLength = buffer.Length - headerLength;

				_header = new byte[headerLength];
				_body = new byte[bodyLength];
				Array.Copy(buffer, 0, _header, 0, headerLength);
				Array.Copy(buffer, headerLength, _body, 0, bodyLength);
				break;
			}
		}

		public byte[] Header
		{
			get { return _header; }
		}

		public byte[] Body
		{
			get { return _body; }
		}
	}
}