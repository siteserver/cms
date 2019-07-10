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

        Task<Content> CalculateAsync(int sequence, Content contentInfo, List<ContentColumn> columns, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns);

        bool IsCreatable(Channel channelInfo, Content contentInfo);
    }
}