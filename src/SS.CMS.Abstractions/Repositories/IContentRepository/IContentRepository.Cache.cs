using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        Task RemoveCacheBySiteIdAsync(string tableName, int siteId);

        void RemoveCache(string tableName, int channelId);

        void RemoveCountCache(string tableName);

        Task<ContentInfo> CalculateAsync(int sequence, ContentInfo contentInfo, List<ContentColumn> columns, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns);

        bool IsCreatable(ChannelInfo channelInfo, ContentInfo contentInfo);
    }
}