using System;
using System.Net.Sockets;
using System.Text;
using NeonMika.Interfaces;

namespace NeonMika.Requests
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
    */

    public class RequestHelper
    {
        private static ILogger _logger;

        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static void SendTextUtf8(string mimeType, string content, Socket client)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            Send200_OK(client, mimeType, bytes.Length);
            client.Send(bytes, bytes.Length, SocketFlags.None);
        }

        public static void Send200_OK(Socket client, string mimeType)
        {
            var header = "HTTP/1.0 200 OK\r\n"
                         + "Content-Type: " + mimeType + "; charset=utf-8\r\n"
                         + "Connection: close\r\n\r\n";

            try
            {
                var buffer = Encoding.UTF8.GetBytes(header);
                client.Send(buffer, buffer.Length, SocketFlags.None);
            }
            catch (Exception e)
            {
                _logger.Debug(e.Message);
            }
        }

        public static void Send200_OK(Socket client, string mimeType, int contentLength)
        {
            string header;
            if (contentLength > 0)
            {
                header = "HTTP/1.0 200 OK\r\n"
                         + "Content-Type: " + mimeType + "; charset=utf-8\r\n"
                         + "Content-Length: " + contentLength + "\r\n"
                         + "Connection: close\r\n\r\n";
            }
            else
            {
                header = "HTTP/1.0 204 No Content\r\n"
                         + "Content-Type: " + mimeType + "; charset=utf-8\r\n"
                         + "Connection: close\r\n\r\n";
            }

            try
            {
                var buffer = Encoding.UTF8.GetBytes(header);
                client.Send(buffer, buffer.Length, SocketFlags.None);
            }
            catch (Exception e)
            {
                _logger.Debug(e.Message);
            }
        }

        public static void Send400_BadRequest(Socket client)
        {
            const string header = "HTTP/1.1 400 Bad Request\r\n"
                                  + "Content-Length: 0\r\nConnection: close\r\n\r\n";
            var buffer = Encoding.UTF8.GetBytes(header);
            if (client != null)
                client.Send(buffer, buffer.Length, SocketFlags.None);
            _logger.Debug("Sent 400 Bad Request");
        }

        public static void Send405_MethodNotAllowed(Socket client)
        {
            const string header = "HTTP/1.1 405 Method Not Allowed\r\n"
                                  + "Content-Length: 0\r\nConnection: close\r\n\r\n";
            var buffer = Encoding.UTF8.GetBytes(header);
            if (client != null)
                client.Send(buffer, buffer.Length, SocketFlags.None);
            _logger.Debug("Sent 405 Method Not Allowed");
        }

        public static void Send404_NotFound(Socket client)
        {
            const string header = "HTTP/1.1 404 Not Found\r\n"
                                  + "Content-Length: 0\r\nConnection: close\r\n\r\n";
            var buffer = Encoding.UTF8.GetBytes(header);
            if (client != null)
                client.Send(buffer, buffer.Length, SocketFlags.None);
            _logger.Debug("Sent 404 Not Found");
        }

        public static void Send500_Failure(Socket client, string message = "")
        {
            var header = "HTTP/1.1 500 Internal Server Error\r\n"
                         + "Content-Length: " + message.Length + "\r\n"
                         + "Connection: close\r\n\r\n"
                         + message;
            var buffer = Encoding.UTF8.GetBytes(header);
            if (client != null)
                client.Send(buffer, buffer.Length, SocketFlags.None);
            _logger.Debug("Sent 500 Internal Server Error");
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
            catch (Exception ex1)
            {
                _logger.Debug("Error on sending data to client / Closing Client / " + ex1);
                try
                {
                    client.Close();
                }
                catch (Exception ex2)
                {
                    _logger.Debug("Error on closing Client / " + ex2);
                }
            }

            return ret;
        }

        public static string GetMimeType(string filename)
        {
            string result;
            var dot = filename.LastIndexOf('.');
            var ext = (dot >= 0) ? filename.Substring(dot + 1) : string.Empty;
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