using System.Collections.Generic;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Repositories
{
    public partial interface IChannelRepository
    {
        Dictionary<int, ChannelInfo> GetChannelInfoDictionaryBySiteId(int siteId);

        void RemoveCacheBySiteId(int siteId);

        void UpdateCache(int siteId, ChannelInfo channelInfo);

        ChannelInfo GetChannelInfo(int siteId, int channelId);

        int GetChannelId(int siteId, int channelId, string channelIndex, string channelName);

        int GetChannelIdByIndexName(int siteId, string indexName);

        int GetChannelIdByParentIdAndChannelName(int siteId, int parentId, string channelName, bool recursive);

        List<ChannelInfo> GetChannelInfoList(int siteId);

        List<int> GetChannelIdList(int siteId);

        Dictionary<int, ChannelInfo> GetChannelInfoDictionary(int siteId);

        List<int> GetChannelIdList(int siteId, string channelGroup);

        List<int> GetChannelIdList(ChannelInfo channelInfo, ScopeType scopeType);

        List<int> GetChannelIdList(ChannelInfo channelInfo, ScopeType scopeType, string group, string groupNot, string contentModelPluginId);

        bool IsExists(int siteId, int channelId);

        bool IsExists(ISiteRepository siteRepository, int channelId);

        int GetChannelIdByParentsCount(int siteId, int channelId, int parentsCount);

        string GetTableName(IPluginManager pluginManager, SiteInfo siteInfo, int channelId);

        string GetTableName(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo);

        string GetTableName(IPluginManager pluginManager, SiteInfo siteInfo, string pluginId);

        bool IsContentModelPlugin(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo nodeInfo);

        string GetNodeTreeLastImageHtml(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo nodeInfo);

        int GetParentId(int siteId, int channelId);

        string GetParentsPath(int siteId, int channelId);

        int GetTopLevel(int siteId, int channelId);

        string GetChannelName(int siteId, int channelId);

        string GetChannelNameNavigation(int siteId, int channelId);

        string GetSelectText(SiteInfo siteInfo, ChannelInfo channelInfo, IUserManager userManager, bool[] isLastNodeArray, bool isShowContentNum);

        string GetContentAttributesOfDisplay(int siteId, int channelId);

        bool IsAncestorOrSelf(int siteId, int parentId, int childId);

        List<KeyValuePair<int, string>> GetChannels(int siteId, IUserManager userManager, params string[] channelPermissions);

        bool IsCreatable(SiteInfo siteInfo, ChannelInfo channelInfo);
    }

}