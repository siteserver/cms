using System;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Features
{
    public interface IJsonApi : IPlugin
    {
        Func<IRequestContext, object> JsonGet { get; }
        Func<IRequestContext, string, object> JsonGetWithName { get; }
        Func<IRequestContext, string, int, object> JsonGetWithNameAndId { get; }

        Func<IRequestContext, object> JsonPost { get; }
        Func<IRequestContext, string, object> JsonPostWithName { get; }
        Func<IRequestContext, string, int, object> JsonPostWithNameAndId { get; }

        Func<IRequestContext, object> JsonPut { get; }
        Func<IRequestContext, string, object> JsonPutWithName { get; }
        Func<IRequestContext, string, int, object> JsonPutWithNameAndId { get; }

        Func<IRequestContext, object> JsonDelete { get; }
        Func<IRequestContext, string, object> JsonDeleteWithName { get; }
        Func<IRequestContext, string, int, object> JsonDeleteWithNameAndId { get; }

        Func<IRequestContext, object> JsonPatch { get; }
        Func<IRequestContext, string, object> JsonPatchWithName { get; }
        Func<IRequestContext, string, int, object> JsonPatchWithNameAndId { get; }
    }
}
