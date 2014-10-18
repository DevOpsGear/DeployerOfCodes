using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Deployer.Services.Micro.Web
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
