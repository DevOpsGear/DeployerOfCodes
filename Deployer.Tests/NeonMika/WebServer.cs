using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NeonMika.Requests;
using NeonMika.Responses;
using NeonMika.Interfaces;

namespace NeonMika
{
    public class WebServer
    {
        private readonly int _port;
        private readonly INeonLogger _logger;
        private readonly INeonGarbage _neonGarbage;
        private readonly ArrayList _responses;
        private readonly Socket _listeningSocket;
        private readonly Thread _requestThread;

        public WebServer(INeonLogger logger, INeonGarbage neonGarbage, int port = 80)
        {
            _port = port;
            _logger = logger;
            _neonGarbage = neonGarbage;

            _logger.Debug("Starting web server");

            _responses = new ArrayList();
            _listeningSocket = SetupListeningSocket();

            _requestThread = new Thread(WaitForNetworkRequest);
            _requestThread.Start();

            _logger.Debug("Webserver is running");
        }

        public void AddResponse(Responder responder)
        {
            _responses.Add(responder);
        }

        private Socket SetupListeningSocket()
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(new IPEndPoint(IPAddress.Any, _port));
            sock.Listen(5);
            return sock;
        }

        private void WaitForNetworkRequest()
        {
            while (true)
            {
                try
                {
                    using (var clientSocket = _listeningSocket.Accept())
                    {
                        var availableBytes = AwaitAvailableBytes(clientSocket, Settings.MAX_HEADER_SIZE);
                        if (availableBytes <= 0)
                            continue;

                        var headerBody = ReadAndBreakApart(clientSocket, availableBytes);
                        var longBody = new ClientRequestBody(headerBody.Body, clientSocket);

                        using (var request = new Request(headerBody.Header, longBody, clientSocket))
                        {
                            _logger.Debug(" * Client connected / URL: " + request.Url + " / Initial byte count: " +
                                          availableBytes);
                            SendResponse(request);
                        }

                        try
                        {
                            clientSocket.Close();
                        }
                        catch (Exception ex)
                        {
                            _logger.Debug(ex.ToString());
                        }

                        _logger.Debug(" * Request finished");
                        _neonGarbage.Collect();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex.Message);
                }
            }
// ReSharper disable FunctionNeverReturns
        }

// ReSharper restore FunctionNeverReturns

        private static int AwaitAvailableBytes(Socket clientSocket, int maxToWaitFor)
        {
            var availableBytes = 0;
            do
            {
                Thread.Sleep(Settings.SLEEP_WAIT_FOR_SOCKET_DATA);
                var newAvBytes = clientSocket.Available - availableBytes;
                if (newAvBytes == 0)
                    break;

                availableBytes += newAvBytes;
                if (availableBytes >= maxToWaitFor)
                    break;
            } while (true); // Repeat until enough bytes have arrived

            return availableBytes;
        }

        private static HeaderBody ReadAndBreakApart(Socket clientSocket, int availableBytes)
        {
            var bufferSize = availableBytes > Settings.MAX_HEADER_SIZE ? Settings.MAX_HEADER_SIZE : availableBytes;
            var buffer = new byte[bufferSize];
            var actualByteCount = clientSocket.Receive(buffer, buffer.Length, SocketFlags.None);
            return new HeaderBody(buffer, actualByteCount);
        }

        private void SendResponse(Request e)
        {
            foreach (Responder resp in _responses)
            {
                if (resp.CanRespond(e))
                {
                    if (!resp.SendResponse(e))
                        _logger.Debug("Sending response failed");
                    return;
                }
            }

            RequestHelper.Send404_NotFound(e.Client);
        }
    }

}