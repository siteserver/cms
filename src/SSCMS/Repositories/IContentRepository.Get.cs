using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Datory;
using SqlKata;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Repositories
{
    public partial interface IContentRepository
    {
        Task<int> GetMaxTaxisAsync(Site site, Channel channel, bool isTop);

        Task<int> GetFirstContentIdAsync(Site site, IChannelSummary channel);

        List<(int AdminId, int AddCount, int UpdateCount)> GetDataSetOfAdminExcludeRecycle(string tableName,
            int siteId, DateTime begin, DateTime end);

        Task<int> GetCountOfContentUpdateAsync(string tableName, int siteId, int channelId, ScopeType scope,
            DateTime begin, DateTime end, int adminId);

        Task<List<int>> GetContentIdsBySameTitleAsync(Site site, Channel channel, string title);

        Task<int> GetCountOfContentAddAsync(string tableName, int siteId, int channelId, ScopeType scope,
            DateTime begin, DateTime end, int adminId, bool? checkedState);

        Task<List<ContentSummary>> GetSummariesAsync(string tableName, Query query);

        Task<int> GetCountAsync(string tableName, Query query);

        Task<Query> GetQueryByStlSearchAsync(IDatabaseManager databaseManager, bool isAllSites, string siteName, string siteDir, string siteIds,
            string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute,
            string dateFrom, string dateTo, string since, int siteId, List<string> excludeAttributes,
            NameValueCollection form);

        Task<string> GetNewContentTableNameAsync();

        Task<string> CreateNewContentTableAsync();

        Task CreateContentTableAsync(string tableName, List<TableColumn> columns);
    }
}
