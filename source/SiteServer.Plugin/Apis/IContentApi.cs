using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Apis
{
    public interface IContentApi
    {
        IContentInfo GetContentInfo(int publishmentSystemId, int channelId, int contentId);
    }
}
