using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Repositories
{
    public partial interface IChannelRepository
    {
        Task<ChannelInfo> GetChannelInfoAsync(int channelId);

        Task<int> GetSiteIdAsync(int channelId);

        Task<int> GetChannelIdAsync(int siteId, int channelId, string channelIndex, string channelName);

        Task<int> GetChannelIdByIndexNameAsync(int siteId, string indexName);

        Task<int> GetChannelIdByParentIdAndChannelNameAsync(int siteId, int parentId, string channelName, bool recursive);

        Task<List<int>> GetChannelIdListAsync(int siteId);

        Task<List<int>> GetChannelIdListAsync(int siteId, string channelGroup);

        Task<List<int>> GetChannelIdListAsync(ChannelInfo channelInfo, ScopeType scopeType);

        Task<List<int>> GetChannelIdListAsync(ChannelInfo channelInfo, ScopeType scopeType, string group, string groupNot, string contentModelPluginId);

        Task<bool> IsExistsAsync(int channelId);

        Task<int> GetChannelIdByParentsCountAsync(int siteId, int channelId, int parentsCount);

        Task<string> GetTableNameAsync(IPluginManager pluginManager, SiteInfo siteInfo, int channelId);

        Task<string> GetTableNameAsync(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo);

        Task<string> GetTableNameAsync(IPluginManager pluginManager, SiteInfo siteInfo, string pluginId);

        Task<bool> IsContentModelPluginAsync(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo);

        Task<string> GetParentsPathAsync(int channelId);

        Task<int> GetTopLevelAsync(int channelId);

        Task<string> GetChannelNameAsync(int channelId);

        Task<string> GetChannelNameNavigationAsync(int siteId, int channelId);

        Task<string> GetContentAttributesOfDisplayAsync(int siteId, int channelId);

        Task<bool> IsAncestorOrSelfAsync(int parentId, int childId);

        Task<bool> IsCreatableAsync(SiteInfo siteInfo, ChannelInfo channelInfo);
    }

}