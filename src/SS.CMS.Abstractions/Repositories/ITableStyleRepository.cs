using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Services;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface ITableStyleRepository : IRepository
    {
        int Insert(TableStyleInfo styleInfo);

        void Update(TableStyleInfo info, bool deleteAndInsertStyleItems = true);

        void Delete(int relatedIdentity, string tableName, string attributeName);

        void Delete(List<int> relatedIdentities, string tableName);

        //cache

        string GetKey(int relatedIdentity, string tableName, string attributeName);

        List<TableStyleInfo> GetStyleInfoList(string tableName, List<int> relatedIdentities);

        List<TableStyleInfo> GetSiteStyleInfoList(int siteId);

        List<TableStyleInfo> GetChannelStyleInfoList(ChannelInfo channelInfo);

        List<TableStyleInfo> GetContentStyleInfoList(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo);

        List<TableStyleInfo> GetUserStyleInfoList();

        IDictionary<string, object> GetDefaultAttributes(List<TableStyleInfo> styleInfoList);

        void ClearCache();

        //relatedIdentities从大到小，最后是0
        TableStyleInfo GetTableStyleInfo(string tableName, string attributeName, List<int> relatedIdentities);

        TableStyleInfo GetTableStyleInfo(int id);

        bool IsExists(int relatedIdentity, string tableName, string attributeName);

        Dictionary<string, List<TableStyleInfo>> GetTableStyleInfoWithItemsDictinary(string tableName, List<int> allRelatedIdentities);

        string GetValidateInfo(TableStyleInfo styleInfo);

        List<int> GetRelatedIdentities(int siteId);

        List<int> GetRelatedIdentities(ChannelInfo channelInfo);

        List<int> EmptyRelatedIdentities { get; }
    }
}