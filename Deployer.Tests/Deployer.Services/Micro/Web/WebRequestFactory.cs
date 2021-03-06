﻿using System;
using System.Net;

namespace Deployer.Services.Micro.Web
{
    public class WebRequestFactory : IWebRequestFactory
    {
        public IWebRequest CreateRequest(string apiRoot, string apiEndpoint, string method)
        {
            var req = WebRequest.Create(apiRoot + apiEndpoint) as HttpWebRequest;
            if (req == null)
                throw new Exception("System error - could not create web request");
            req.Method = method;
            return new WebRequestWrapper(req);
        }
    }
}