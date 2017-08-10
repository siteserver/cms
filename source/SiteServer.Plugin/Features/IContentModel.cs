using System.Collections.Generic;

namespace SiteServer.Plugin.Features
{
    public interface IContentModel: IPlugin
    {
        List<PluginContentLink> ContentLinks { get; }

        void AfterContentAdded(int siteId, int channelId, int contentId);

        void AfterContentTranslated(int siteId, int channelId, int contentId, int targetSiteId, int targetChannelId,
            int targetContentId);

        void AfterContentDeleted(int siteId, int channelId, int contentId);

        bool IsCustomContentTable { get; }

        string CustomContentTableName { get; }

        List<PluginTableColumn> CustomContentTableColumns { get; }
    }
}
