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
    public class WebServer : IWebServer
    {
        private ILogger _logger;
        private IGarbage _garbage;
        private readonly int _port;
        private ArrayList _responses;
        private Socket _listeningSocket;
        private Thread _requestThread;
        private bool _running;
        private bool _stopSignal;

        public WebServer(ILogger logger, IGarbage garbage, int port = 80)
        {
            _logger = logger;
            _garbage = garbage;
            _port = port;

            _logger.Debug("Starting web server");

            _responses = new ArrayList();
            _listeningSocket = SetupListeningSocket();

            _requestThread = new Thread(WaitForNetworkRequest);
            _requestThread.Start();

            _logger.Debug("Web server is running");
        }

        public void AddResponse(Responder responder)
        {
            _responses.Add(responder);
        }

        // TODO: Handle multi-threading?
        public bool Stop()
        {
            _stopSignal = true;
            ShutDownListeningSocket();
            while (_running)
                Thread.Sleep(250);
            return true;
        }

        public void Dispose()
        {
            _logger = null;
            _garbage = null;
            _responses = null;
            //_listeningSocket.Dispose();
            _listeningSocket = null;
            if (_requestThread.IsAlive)
                _requestThread.Abort();
            _requestThread = null;
        }

        private Socket SetupListeningSocket()
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Bind(new IPEndPoint(IPAddress.Any, _port));
            sock.Listen(5);
            //sock.LingerState = new LingerOption(false, 0);
            return sock;
        }

        // http://stackoverflow.com/questions/1097977/net-stop-listening-socket
        private void ShutDownListeningSocket()
        {
            // _listeningSocket.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
            //_listeningSocket.Shutdown(SocketShutdown.Both);
            //_listeningSocket.Disconnect(false);
            _listeningSocket.Close();
        }

        private void WaitForNetworkRequest()
        {
            while (ServerShouldBeUp)
            {
                _running = true;
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
                        _garbage.Collect();
                    }
                }
                catch (SocketException ex)
                {
                    if (ServerShouldBeUp)
                        _logger.Debug(ex.Message);
                }
                catch (Exception ex)
                {
                    _logger.Debug(ex.Message);
                }
            }
            _logger.Debug("Web server has exited");
            _running = false;
        }

        // TODO: Lock for multi-threading?
        private bool ServerShouldBeUp
        {
            get { return !_stopSignal; }
        }

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
                if (!resp.CanRespond(e)) continue;
                if (!resp.SendResponse(e))
                    _logger.Debug("Sending response failed");
                return;
            }

            RequestHelper.Send404_NotFound(e.Client);
        }
    }
}