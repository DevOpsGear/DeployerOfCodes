using System;
using NeonMika.Responses;

namespace NeonMika
{
    public interface IWebServer : IDisposable
    {
        void AddResponse(Responder responder);
        bool Stop();
    }
}