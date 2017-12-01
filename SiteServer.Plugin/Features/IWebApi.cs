using System;
using System.Net.Http;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Features
{
    public interface IWebApi : IPlugin
    {
        Func<IRequestContext, object> JsonGet { get; }
        Func<IRequestContext, string, object> JsonGetWithName { get; }
        Func<IRequestContext, string, string, object> JsonGetWithNameAndId { get; }

        Func<IRequestContext, object> JsonPost { get; }
        Func<IRequestContext, string, object> JsonPostWithName { get; }
        Func<IRequestContext, string, string, object> JsonPostWithNameAndId { get; }

        Func<IRequestContext, object> JsonPut { get; }
        Func<IRequestContext, string, object> JsonPutWithName { get; }
        Func<IRequestContext, string, string, object> JsonPutWithNameAndId { get; }

        Func<IRequestContext, object> JsonDelete { get; }
        Func<IRequestContext, string, object> JsonDeleteWithName { get; }
        Func<IRequestContext, string, string, object> JsonDeleteWithNameAndId { get; }

        Func<IRequestContext, object> JsonPatch { get; }
        Func<IRequestContext, string, object> JsonPatchWithName { get; }
        Func<IRequestContext, string, string, object> JsonPatchWithNameAndId { get; }

        Func<IRequestContext, HttpResponseMessage> HttpGet { get; }
        Func<IRequestContext, string, HttpResponseMessage> HttpGetWithName { get; }
        Func<IRequestContext, string, string, HttpResponseMessage> HttpGetWithNameAndId { get; }

        Func<IRequestContext, HttpResponseMessage> HttpPost { get; }
        Func<IRequestContext, string, HttpResponseMessage> HttpPostWithName { get; }
        Func<IRequestContext, string, string, HttpResponseMessage> HttpPostWithNameAndId { get; }

        Func<IRequestContext, HttpResponseMessage> HttpPut { get; }
        Func<IRequestContext, string, HttpResponseMessage> HttpPutWithName { get; }
        Func<IRequestContext, string, string, HttpResponseMessage> HttpPutWithNameAndId { get; }

        Func<IRequestContext, HttpResponseMessage> HttpDelete { get; }
        Func<IRequestContext, string, HttpResponseMessage> HttpDeleteWithName { get; }
        Func<IRequestContext, string, string, HttpResponseMessage> HttpDeleteWithNameAndId { get; }

        Func<IRequestContext, HttpResponseMessage> HttpPatch { get; }
        Func<IRequestContext, string, HttpResponseMessage> HttpPatchWithName { get; }
        Func<IRequestContext, string, string, HttpResponseMessage> HttpPatchWithNameAndId { get; }
    }
}
