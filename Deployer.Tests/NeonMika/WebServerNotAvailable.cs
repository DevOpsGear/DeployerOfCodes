using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NeonMika.Requests;
using NeonMika.Responses;
using NeonMika.Interfaces;

namespace NeonMika
{
    public class WebServerNotAvailable : IWebServer
    {
        private ILogger _logger;
        private readonly int _port;
        private Socket _listeningSocket;
        private Thread _requestThread;
        private bool _running;
        private bool _stopSignal;

        public WebServerNotAvailable(ILogger logger, int port = 80)
        {
            _logger = logger;
            _port = port;

            _logger.Debug("Starting NOT AVAILABLE web server");

            _listeningSocket = SetupListeningSocket();

            _requestThread = new Thread(WaitForNetworkRequest);
            _requestThread.Start();

            _logger.Debug("NOT AVAILABLE web server is running");
        }

        public void AddResponse(Responder responder)
        {
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
                        RequestHelper.Send503_ServiceUnavailable(clientSocket);
                        clientSocket.Close();
                    }
                }
                catch (Exception ex)
                {
                    if (ServerShouldBeUp)
                        _logger.Debug(ex.Message);
                }
            }
            _logger.Debug("NOT AVAILABLE web server has exited");
            _running = false;
        }

        private bool ServerShouldBeUp
        {
            get { return !_stopSignal; }
        }
    }
}