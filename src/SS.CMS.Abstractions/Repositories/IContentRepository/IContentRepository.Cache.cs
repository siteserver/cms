using System;
using System.Collections.Generic;
using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Repositories
{
    public partial interface IContentRepository
    {
        void RemoveCacheBySiteId(string tableName, int siteId);

        void RemoveCache(string tableName, int channelId);

        void RemoveCountCache(string tableName);

        void InsertCache(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo);

        void UpdateCache(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfoToUpdate);

        List<ContentColumn> GetContentColumns(SiteInfo siteInfo, ChannelInfo channelInfo, bool includeAll);

        ContentInfo Calculate(int sequence, ContentInfo contentInfo, List<ContentColumn> columns, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns);

        bool IsCreatable(ChannelInfo channelInfo, ContentInfo contentInfo);
    }
}