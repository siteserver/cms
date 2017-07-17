using System.Collections.Generic;

namespace SiteServer.Plugin.Hooks
{
    public interface IContentExtra : IHooks
    {
        List<PluginContentLink> ContentLinks { get; }

        void AfterContentTranslated(int siteId, int channelId, int contentId, int targetSiteId, int targetChannelId,
            int targetContentId);

        void AfterContentDeleted(int siteId, int channelId, int contentId);
    }
}
