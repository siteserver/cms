using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Repositories
{
    public interface IChannelRepository : IRepository
    {
        Task<int> InsertAsync(Channel channel);

        Task UpdateAsync(Channel channel);

        Task UpdateExtendAsync(Channel channel);

        Task<Channel> DeleteAsync(int siteId, int id);

        Task<Channel> GetChannelByLastAddDateAsync(int id);

        Task<Channel> GetChannelByTaxisAsync(int id);

        Task<int> GetIdByParentIdAndTaxisAsync(int parentId, int taxis, bool isNextChannel);

        Task<int> GetIdAsync(int siteId, string orderString);

        Task<string> GetOrderStringInSiteAsync(int id);

        Task<IEnumerable<string>> GetIndexNameListAsync(int siteId);

        Task<int> GetCountAsync(int id);

        Task<int> GetSequenceAsync(int siteId, int id);

        Task<IEnumerable<int>> GetIdListByTotalNumAsync(int siteId, int id, TaxisType taxisType, ScopeType scopeType, string groupChannel, string groupChannelNot, bool? isImage, int totalNum);

        Task<IEnumerable<KeyValuePair<int, Channel>>> GetContainerChannelListAsync(int siteId, int id, string group, string groupNot, bool? isImage, int startNum, int totalNum, TaxisType taxisType, ScopeType scopeType, bool isTotal);

        Task<IEnumerable<string>> GetContentModelPluginIdListAsync();

        Task<IEnumerable<int>> GetIdListAsync(Template template);

        Task<IEnumerable<int>> GetDescendantIdsAsync(int siteId, int parentId);

        Task<IEnumerable<int>> GetChildrenIdsAsync(int siteId, int parentId);

        Task<string> GetSourceNameAsync(int sourceId);

        Task<Channel> GetChannelAsync(int id);

        Task<int> GetSiteIdAsync(int id);

        Task<int> GetIdAsync(int siteId, int id, string channelIndex, string channelName);

        Task<int> GetIdByIndexNameAsync(int siteId, string indexName);

        Task<int> GetIdByParentIdAndChannelNameAsync(int siteId, int parentId, string channelName, bool recursive);

        Task<List<int>> GetIdListAsync(int siteId);

        Task<List<int>> GetIdListAsync(int siteId, string channelGroup);

        Task<List<int>> GetIdListAsync(Channel channel, ScopeType scopeType);

        Task<List<int>> GetIdListAsync(Channel channel, ScopeType scopeType, string group, string groupNot, string contentModelPluginId);

        Task<bool> IsExistsAsync(int id);

        string GetTableName(Site site, Channel channel);

        Task<string> GetTableNameAsync(Site site, int channelId);

        Task<IContentRepository> GetContentRepositoryAsync(Site site, int channelId);

        IContentRepository GetContentRepository(Site site, Channel channel);

        Task<IContentRepository> GetContentRepositoryAsync(int siteId);

        IContentRepository GetContentRepository(Site site);

        // Task<string> GetTableNameAsync(IPluginManager pluginManager, Site site, int id);

        // Task<string> GetTableNameAsync(IPluginManager pluginManager, Site site, Channel channel);

        // Task<string> GetTableNameAsync(IPluginManager pluginManager, Site site, string pluginId);

        Task<bool> IsContentModelPluginAsync(IPluginManager pluginManager, Site site, Channel channel);

        Task<string> GetParentsPathAsync(int id);

        Task<int> GetTopLevelAsync(int id);

        Task<string> GetChannelNameAsync(int id);

        Task<string> GetChannelNameNavigationAsync(int siteId, int id);

        Task<string> GetContentAttributesOfDisplayAsync(int siteId, int id);

        Task<bool> IsAncestorOrSelfAsync(int parentId, int childId);

        Task<bool> IsCreatableAsync(Site site, Channel channel);

        Task<IList<Channel>> GetChannelListAsync(int siteId, int parentId);

        List<int> GetParentIds(Channel channel);

        int GetParentsCount(Channel channel);
    }
}