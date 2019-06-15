using System.Collections.Generic;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface IChannelRepository : IRepository
    {
        int Insert(int siteId, int parentId, string channelName, string indexName, string contentModelPluginId,
            string contentRelatedPluginIds, int channelTemplateId, int contentTemplateId);

        int Insert(ChannelInfo channelInfo);

        /// <summary>
        /// 添加后台发布节点
        /// </summary>
        int InsertSiteInfo(ChannelInfo channelInfo, SiteInfo siteInfo, string administratorName);

        void Update(ChannelInfo channelInfo);

        void UpdateChannelTemplateId(ChannelInfo channelInfo);

        void UpdateContentTemplateId(ChannelInfo channelInfo);

        void UpdateExtend(ChannelInfo channelInfo);

        void UpdateTaxis(int siteId, int selectedId, bool isSubtract);

        void AddGroupNameList(int siteId, int channelId, List<string> groupList);

        void DeleteAll(int siteId);

        void Delete(int siteId, int channelId);

        /// <summary>
        /// 得到最后一个添加的子节点
        /// </summary>
        ChannelInfo GetChannelInfoByLastAddDate(int channelId);

        /// <summary>
        /// 得到第一个子节点
        /// </summary>
        ChannelInfo GetChannelInfoByTaxis(int channelId);

        int GetIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel);

        int GetId(int siteId, string orderString);

        int GetSiteId(int channelId);

        /// <summary>
        /// 在节点树中得到此节点的排序号，以“1_2_5_2”的形式表示
        /// </summary>
        string GetOrderStringInSite(int channelId);

        IList<string> GetIndexNameList(int siteId);

        int GetCount(int channelId);

        int GetSequence(int siteId, int channelId);

        IList<int> GetIdListByTotalNum(int siteId, int channelId, TaxisType taxisType, ScopeType scopeType,
            string groupChannel, string groupChannelNot, bool? isImage, int totalNum);

        Dictionary<int, ChannelInfo> GetChannelInfoDictionaryBySiteId(int siteId);

        IList<KeyValuePair<int, ChannelInfo>> GetContainerChannelList(int siteId, int channelId, string group,
            string groupNot, bool? isImage, int startNum, int totalNum, TaxisType taxisType,
            ScopeType scopeType, bool isTotal);

        IList<string> GetContentModelPluginIdList();

        IList<string> GetAllFilePathBySiteId(int siteId);

        int GetTemplateUseCount(int siteId, int templateId, TemplateType templateType, bool isDefault);

        IList<int> GetChannelIdList(TemplateInfo templateInfo);
    }
}
