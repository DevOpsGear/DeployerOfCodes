using System;
using System.Net.Sockets;
using System.Collections;

namespace NeonMika
{
	public class Request : IDisposable
	{
		private readonly Socket _client;
		private string _method;
		private string _url;
		private Hashtable _getArguments;
		private string _body;
		private Hashtable _headers;

		public Request(char[] header, char[] body, Socket client)
		{
			_client = client;
			_getArguments = new Hashtable();
			_headers = new Hashtable();
			ProcessHeader(header);
			_body = new string(body);
		}

		public Hashtable Headers
		{
			get { return _headers; }
			set { _headers = value; }
		}

		public Hashtable GetArguments
		{
			get { return _getArguments; }
		}

		public string Body
		{
			get { return _body; }
		}

		public string Method
		{
			get { return _method; }
		}

		public string Url
		{
			get { return _url; }
		}

		public Socket Client
		{
			get { return _client; }
		}

		private void ProcessHeader(char[] data)
		{
			for (var i = 0; i < data.Length - 3; i++)
			{
				var replace = false;

				switch (data[i].ToString() + data[i + 1] + data[i + 2])
				{
					case "%5C":
						data[i] = '\\';
						data[i + 1] = '\0';
						data[i + 2] = '\0';
						replace = true;
						break;

					case "%2F":
						data[i] = '/';
						data[i + 1] = '\0';
						data[i + 2] = '\0';
						replace = true;
						break;
				}

				if (!replace)
					continue;
				for (var x = i + 3; x < data.Length; x++)
					if (data[x] != '\0')
					{
						data[x - 2] = data[x];
						data[x] = '\0';
					}
			}

			var content = new string(data);
			var lines = content.Split('\n');

			// Parse the first line of the request: "GET /path/ HTTP/1.1"
			var firstLineSplit = lines[0].Split(' ');
			_method = firstLineSplit[0];
			var path = firstLineSplit[1].Split('?');
			_url = path[0].Substring(1); // Ignore the leading '/'

			_getArguments.Clear();
			if (path.Length > 1)
				ProcessGetParameters(path[1]);

			if (_method == "POST" || _method == "PUT")
			{
				_body = content;
			}
			_headers = Util.Converter.ToHashtable(lines, ": ", 1);
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