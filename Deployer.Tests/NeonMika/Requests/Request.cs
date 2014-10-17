using System;
using System.Net.Sockets;
using System.Collections;
using System.Text;

namespace NeonMika.Requests
{
	public class Request : IDisposable
	{
		private readonly ClientRequestBody _body;
		private readonly Socket _client;
		private string _httpMethod;
		private string _url;
		private Hashtable _getArguments;
		private readonly Hashtable _headers;

		public Request(byte[] header, ClientRequestBody body, Socket client)
		{
			_getArguments = new Hashtable();
			_headers = ProcessHeader(header);
			_body = body;
			_client = client;
		}

		public Hashtable Headers
		{
			get { return _headers; }
		}

		public Hashtable GetArguments
		{
			get { return _getArguments; }
		}

		public ClientRequestBody Body
		{
			get { return _body; }
		}

		public string HttpMethod
		{
			get { return _httpMethod; }
		}

		public string Url
		{
			get { return _url; }
		}

		public Socket Client
		{
			get { return _client; }
		}

		private Hashtable ProcessHeader(byte[] bytes)
		{
			var chars = Encoding.UTF8.GetChars(bytes);
			for (var i = 0; i < chars.Length - 3; i++)
			{
				var replace = false;

				switch (chars[i].ToString() + chars[i + 1] + chars[i + 2])
				{
					case "%5C":
						chars[i] = '\\';
						chars[i + 1] = '\0';
						chars[i + 2] = '\0';
						replace = true;
						break;

					case "%2F":
						chars[i] = '/';
						chars[i + 1] = '\0';
						chars[i + 2] = '\0';
						replace = true;
						break;
				}

				if (!replace)
					continue;
				for (var x = i + 3; x < chars.Length; x++)
					if (chars[x] != '\0')
					{
						chars[x - 2] = chars[x];
						chars[x] = '\0';
					}
			}

			var content = new string(chars);
			var lines = content.Split('\n');

			// Parse the first line of the request: "GET /path/ HTTP/1.1"
			var firstLineSplit = lines[0].Split(' ');
			_httpMethod = firstLineSplit[0].ToUpper();
			var path = firstLineSplit[1].Split('?');
			_url = path[0].Substring(1); // Ignore the leading '/'

			if (path.Length > 1)
				ProcessGetParameters(path[1]);

			return Util.Converter.ToHashtable(lines, ": ", 1);
		}

		private void ProcessGetParameters(string parameters)
		{
			var urlArguments = parameters.Split('&');
			_getArguments = Util.Converter.ToHashtable(urlArguments, "=");
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_headers != null)
				_headers.Clear();

			if (_getArguments != null)
				_getArguments.Clear();
		}

		#endregion
	}
}