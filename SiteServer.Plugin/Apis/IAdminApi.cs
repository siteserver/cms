using System.Collections.Generic;

namespace SiteServer.Plugin.Apis
{
    public interface IAdminApi
    {
        bool IsAdminNameExists(string adminName);

        string AdminName { get; }

        bool IsPluginAuthorized { get; }

        bool IsSiteAuthorized(int publishmentSystemId);

        bool HasSitePermissions(int publishmentSystemId, params string[] sitePermissions);

        bool HasChannelPermissions(int publishmentSystemId, int channelId, params string[] channelPermissions);
    }
}
