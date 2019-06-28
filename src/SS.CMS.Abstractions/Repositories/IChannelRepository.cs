using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IChannelRepository : IRepository
    {
        Task<int> InsertAsync(int siteId, int parentId, string channelName, string indexName, string contentModelPluginId, string contentRelatedPluginIds, int channelTemplateId, int contentTemplateId);

        Task<int> InsertAsync(ChannelInfo channelInfo);

        int InsertSiteInfo(ChannelInfo channelInfo, SiteInfo siteInfo, string administratorName);

        Task UpdateAsync(ChannelInfo channelInfo);

        Task UpdateChannelTemplateIdAsync(ChannelInfo channelInfo);

        Task UpdateContentTemplateIdAsync(ChannelInfo channelInfo);

        Task UpdateExtendAsync(ChannelInfo channelInfo);

        Task UpdateTaxisAsync(int siteId, int selectedId, bool isSubtract);

        Task AddGroupNameListAsync(int siteId, int channelId, List<string> groupList);

        Task DeleteAllAsync(int siteId);

        Task DeleteAsync(int siteId, int channelId);

        ChannelInfo GetChannelInfoByLastAddDate(int channelId);

        ChannelInfo GetChannelInfoByTaxis(int channelId);

        int GetIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel);

        int GetId(int siteId, string orderString);

        int GetSiteId(int channelId);

        string GetOrderStringInSite(int channelId);

        IList<string> GetIndexNameList(int siteId);

        int GetCount(int channelId);

        Task<int> GetSequenceAsync(int siteId, int channelId);

        Task<IList<int>> GetIdListByTotalNumAsync(int siteId, int channelId, TaxisType taxisType, ScopeType scopeType,
            string groupChannel, string groupChannelNot, bool? isImage, int totalNum);

        Task<IList<KeyValuePair<int, ChannelInfo>>> GetContainerChannelListAsync(int siteId, int channelId,
            string group, string groupNot, bool? isImage, int startNum, int totalNum, TaxisType taxisType,
            ScopeType scopeType, bool isTotal);

        IList<string> GetContentModelPluginIdList();

        IList<string> GetAllFilePathBySiteId(int siteId);

        int GetTemplateUseCount(int siteId, int templateId, TemplateType templateType, bool isDefault);

        IList<int> GetChannelIdList(TemplateInfo templateInfo);

        Task<string> GetSourceNameAsync(int sourceId);
    }
}
