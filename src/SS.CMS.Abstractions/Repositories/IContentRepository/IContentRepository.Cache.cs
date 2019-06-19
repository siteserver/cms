using System;
using System.Collections.Generic;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        void RemoveCacheBySiteId(string tableName, int siteId);

        void RemoveCache(string tableName, int channelId);

        void RemoveCountCache(string tableName);

        void InsertCache(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo);

        void UpdateCache(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfoToUpdate);

        ContentInfo Calculate(int sequence, ContentInfo contentInfo, List<ContentColumn> columns, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns);

        bool IsCreatable(ChannelInfo channelInfo, ContentInfo contentInfo);
    }
}