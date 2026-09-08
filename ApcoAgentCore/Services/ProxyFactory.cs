using ApcoAgentCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ApcoAgentCore.Services
{
    public static class ProxyFactory
    {
        public static HttpClient Create(LLM model)
        {
            if (!model.UseProxy)
                return new HttpClient();

            var proxyAddress = $"http://{model.ProxyHost}:{model.ProxyPort}";

            var proxy = new WebProxy(proxyAddress);

            var handler = new HttpClientHandler
            {
                Proxy = proxy,
                UseProxy = true
            };

            return new HttpClient(handler);
        }
    }
}
