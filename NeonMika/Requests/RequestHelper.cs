using System;
using System.Net.Sockets;
using System.Text;
using Microsoft.SPOT;

namespace NeonMika.Requests
{
	public class RequestHelper
	{
		public static void SendTextUtf8(string mimeType, string content, Socket client)
		{
			var bytes = Encoding.UTF8.GetBytes(content);
			Send200_OK(mimeType, bytes.Length, client);
			client.Send(bytes, bytes.Length, SocketFlags.None);
		}

		public static void Send200_OK(string mimeType, int contentLength, Socket client)
		{
			/*
            StringBuilder headerBuilder = new StringBuilder();
            headerBuilder.Append("HTTP/1.0 200 OK\r\n");
            headerBuilder.Append("Content-Type: ");
            headerBuilder.Append(MimeType);
            headerBuilder.Append("; charset=utf-8\r\n");
            headerBuilder.Append("Content-Length: ");
            headerBuilder.Append(ContentLength.ToString());
            headerBuilder.Append("\r\n");
            headerBuilder.Append("Connection: close\r\n\r\n");
             * */

			string header;
			if (contentLength > 0)
				header = "HTTP/1.0 200 OK\r\n" + "Content-Type: " + mimeType + "; charset=utf-8\r\n" + "Content-Length: " +
				         contentLength.ToString() + "\r\n" + "Connection: close\r\n\r\n";
			else
				header = "HTTP/1.0 200 OK\r\n" + "Content-Type: " + mimeType + "; charset=utf-8\r\n" + "Connection: close\r\n\r\n";

			try
			{
				client.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
			}
			catch (Exception e)
			{
				Debug.Print(e.Message);
			}
		}

		public static void Send404_NotFound(Socket client)
		{
			const string header = "HTTP/1.1 404 Not Found\r\n"
			                      + "Content-Length: 0\r\nConnection: close\r\n\r\n"
			                      + "<html><body><head><title>NeonMika.Webserver is sorry</title></head>"
			                      + "<h1>NeonMika.Webserver is sorry!</h1>"
			                      + "<h2>The file or webmethod you were looking for was not found :/</h2></body></html>";
			if (client != null)
				client.Send(Encoding.UTF8.GetBytes(header), header.Length, SocketFlags.None);
			Debug.Print("Sent 404 Not Found");
		}

		public static int SendData(Socket client, byte[] data)
		{
			var ret = 0;
			try
			{
				if (IsSocketConnected(client))
					ret = client.Send(data, data.Length, SocketFlags.None);
				else
				{
					client.Close();
				}
			}
			catch (Exception)
			{
				Debug.Print("Error on sending data to client / Closing Client");
				try
				{
					client.Close();
				}
				catch
				{
				}
			}

			return ret;
		}

		public static string GetMimeType(string filename)
		{
			string result;
			var dot = filename.LastIndexOf('.');

			string ext = (dot >= 0) ? filename.Substring(dot + 1) : string.Empty;
			switch (ext.ToLower())
			{
				case "txt":
					result = "text/plain";
					break;
				case "htm":
				case "html":
					result = "text/html";
					break;
				case "js":
					result = "text/javascript";
					break;
				case "css":
					result = "text/css";
					break;
				case "xml":
				case "xsl":
					result = "text/xml";
					break;
				case "jpg":
				case "jpeg":
					result = "image/jpeg";
					break;
				case "gif":
					result = "image/gif";
					break;
				case "png":
					result = "image/png";
					break;
				case "ico":
					result = "x-icon";
					break;
				case "mid":
					result = "audio/mid";
					break;
				default:
					result = "application/octet-stream";
					break;
			}
			return result;
		}

		public static bool IsSocketConnected(Socket s)
		{
			var part1 = s.Poll(1000, SelectMode.SelectRead);
			var part2 = (s.Available == 0);
			return !(part1 & part2);
		}
	}
}