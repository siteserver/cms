using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Repositories
{
    public partial interface IChannelRepository
    {
        Task<int> GetSiteIdAsync(int channelId);

        Task<Channel> GetChannelByLastAddDateAsyncTask(int siteId, int parentId);

        Task<Channel> GetChannelByTaxisAsync(int siteId, int parentId);

        Task<int> GetIdByParentIdAndTaxisAsync(int siteId, int parentId, int taxis, bool isNextChannel);

        Task<List<string>> GetIndexNamesAsync(int siteId);

        Task<bool> IsIndexNameExistsAsync(int siteId, string indexName);

        Task<int> GetSequenceAsync(int siteId, int channelId);

        Task<Cascade<int>> GetCascadeAsync(Site site, IChannelSummary summary,
            Func<IChannelSummary, Task<object>> func);

        Task<List<Cascade<int>>> GetCascadeChildrenAsync(Site site, int parentId,
            Func<IChannelSummary, Task<object>> func);

        Task<Cascade<int>> GetCascadeAsync(Site site, IChannelSummary summary);

        Task<IList<Channel>> GetChildrenAsync(int siteId, int parentId);

        Task<int> GetChannelIdAsync(int siteId, int channelId, string channelIndex, string channelName);

        Task<int> GetChannelIdByIndexNameAsync(int siteId, string indexName);

        Task<int> GetChannelIdByParentIdAndChannelNameAsync(int siteId, int parentId, string channelName,
            bool recursive);

        Task<List<Channel>> GetChannelsAsync(int siteId);

        Task<List<int>> GetChannelIdsAsync(int siteId);

        Task<List<string>> GetChannelIndexNamesAsync(int siteId);

        Task<List<int>> GetChannelIdsAsync(int siteId, int channelId, ScopeType scopeType);

        Task<List<int>> GetChannelIdsAsync(Channel channel, ScopeType scopeType, string group, string groupNot,
            string contentModelPluginId);

        Task<bool> IsExistsAsync(int channelId);

        Task<string> GetTableNameAsync(Site site, int channelId);

        string GetTableName(Site site, IChannelSummary channel);

        bool IsContentModelPlugin(Site site, Channel node);

        Task<List<string>> GetGroupNamesAsync(int channelId);

        Task<int> GetParentIdAsync(int siteId, int channelId);

        Task<int> GetTopLevelAsync(int siteId, int channelId);

        Task<string> GetChannelNameAsync(int siteId, int channelId);

        Task<string> GetIndexNameAsync(int siteId, int channelId);

        Task<string> GetChannelNameNavigationAsync(int siteId, int channelId);

        Task<string> GetChannelNameNavigationAsync(int siteId, int currentChannelId, int channelId);

        Task<List<int>> GetChannelIdNavigationAsync(int siteId, int channelId);

        Task<bool> IsAncestorOrSelfAsync(int siteId, int parentId, int childId);

        Task<List<KeyValuePair<int, string>>> GetChannelsAsync(int siteId, IAuthManager authManager, params string[] contentPermissions);

        bool IsCreatable(Site site, Channel channel, int count);
    }
}
