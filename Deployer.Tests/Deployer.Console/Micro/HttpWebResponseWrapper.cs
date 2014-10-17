using Deployer.Services.Micro.Web;
using System.IO;
using System.Net;

namespace Deployer.Text.Micro
{
    public class HttpWebResponseWrapper : IHttpWebResponse
    {
        private readonly WebResponse _resp;

        public HttpWebResponseWrapper(WebResponse resp)
        {
            _resp = resp;
        }

        public void Dispose()
        {
            _resp.Dispose();
        }

        public Stream GetResponseStream()
        {
            return _resp.GetResponseStream();
        }
    }
}
