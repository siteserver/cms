using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Repositories
{
    public partial interface IChannelRepository
    {
        Task<Dictionary<int, ChannelInfo>> GetChannelInfoDictionaryBySiteIdAsync(int siteId);

        void RemoveCacheBySiteId(int siteId);

        Task UpdateCacheAsync(int siteId, ChannelInfo channelInfo);

        Task<ChannelInfo> GetChannelInfoAsync(int siteId, int channelId);

        Task<int> GetChannelIdAsync(int siteId, int channelId, string channelIndex, string channelName);

        Task<int> GetChannelIdByIndexNameAsync(int siteId, string indexName);

        Task<int> GetChannelIdByParentIdAndChannelNameAsync(int siteId, int parentId, string channelName, bool recursive);

        Task<List<ChannelInfo>> GetChannelInfoListAsync(int siteId);

        Task<List<int>> GetChannelIdListAsync(int siteId);

        Task<Dictionary<int, ChannelInfo>> GetChannelInfoDictionaryAsync(int siteId);

        Task<List<int>> GetChannelIdListAsync(int siteId, string channelGroup);

        Task<List<int>> GetChannelIdListAsync(ChannelInfo channelInfo, ScopeType scopeType);

        Task<List<int>> GetChannelIdListAsync(ChannelInfo channelInfo, ScopeType scopeType, string group, string groupNot, string contentModelPluginId);

        Task<bool> IsExistsAsync(int siteId, int channelId);

        Task<bool> IsExistsAsync(ISiteRepository siteRepository, int channelId);

        Task<int> GetChannelIdByParentsCountAsync(int siteId, int channelId, int parentsCount);

        Task<string> GetTableNameAsync(IPluginManager pluginManager, SiteInfo siteInfo, int channelId);

        Task<string> GetTableNameAsync(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo);

        Task<string> GetTableNameAsync(IPluginManager pluginManager, SiteInfo siteInfo, string pluginId);

        Task<bool> IsContentModelPluginAsync(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo nodeInfo);

        Task<string> GetNodeTreeLastImageHtmlAsync(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo nodeInfo);

        Task<int> GetParentIdAsync(int siteId, int channelId);

        Task<string> GetParentsPathAsync(int siteId, int channelId);

        Task<int> GetTopLevelAsync(int siteId, int channelId);

        Task<string> GetChannelNameAsync(int siteId, int channelId);

        Task<string> GetChannelNameNavigationAsync(int siteId, int channelId);

        Task<string> GetSelectTextAsync(SiteInfo siteInfo, ChannelInfo channelInfo, IUserManager userManager, bool[] isLastNodeArray, bool isShowContentNum);

        Task<string> GetContentAttributesOfDisplayAsync(int siteId, int channelId);

        Task<bool> IsAncestorOrSelfAsync(int siteId, int parentId, int childId);

        Task<List<KeyValuePair<int, string>>> GetChannelsAsync(int siteId, IUserManager userManager, params string[] channelPermissions);

        Task<bool> IsCreatableAsync(SiteInfo siteInfo, ChannelInfo channelInfo);
    }

}