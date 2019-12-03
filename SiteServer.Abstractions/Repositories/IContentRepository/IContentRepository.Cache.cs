using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IContentRepository
    {
        Task RemoveCacheBySiteIdAsync(string tableName, int siteId);

        Task RemoveCacheAsync(string tableName, int channelId);

        Task RemoveCountCacheAsync(string tableName);

        Task<Content> CalculateAsync(int sequence, Content contentInfo, List<ContentColumn> columns, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns);

        bool IsCreatable(Channel channelInfo, Content contentInfo);
    }
}