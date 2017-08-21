using System;
using System.Web;

namespace SiteServer.Plugin.Features
{
    public interface IHttpApi : IPlugin
    {
        Action<HttpRequest, HttpResponse> HttpGet { get; }
        Action<HttpRequest, HttpResponse, string> HttpGetWithName { get; }
        Action<HttpRequest, HttpResponse, string, int> HttpGetWithNameAndId { get; }

        Action<HttpRequest, HttpResponse> HttpPost { get; }
        Action<HttpRequest, HttpResponse, string> HttpPostWithName { get; }
        Action<HttpRequest, HttpResponse, string, int> HttpPostWithNameAndId { get; }

        Action<HttpRequest, HttpResponse> HttpPut { get; }
        Action<HttpRequest, HttpResponse, string> HttpPutWithName { get; }
        Action<HttpRequest, HttpResponse, string, int> HttpPutWithNameAndId { get; }

        Action<HttpRequest, HttpResponse> HttpDelete { get; }
        Action<HttpRequest, HttpResponse, string> HttpDeleteWithName { get; }
        Action<HttpRequest, HttpResponse, string, int> HttpDeleteWithNameAndId { get; }

        Action<HttpRequest, HttpResponse> HttpPatch { get; }
        Action<HttpRequest, HttpResponse, string> HttpPatchWithName { get; }
        Action<HttpRequest, HttpResponse, string, int> HttpPatchWithNameAndId { get; }
    }
}
