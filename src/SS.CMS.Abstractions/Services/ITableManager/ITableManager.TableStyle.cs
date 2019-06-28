using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface ITableManager
    {
        List<TableStyleInfo> GetStyleInfoList(string tableName, List<int> relatedIdentities);

        List<TableStyleInfo> GetSiteStyleInfoList(int siteId);

        List<TableStyleInfo> GetChannelStyleInfoList(ChannelInfo channelInfo);

        Task<List<TableStyleInfo>> GetContentStyleInfoListAsync(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo);

        List<TableStyleInfo> GetUserStyleInfoList();

        IDictionary<string, object> GetDefaultAttributes(List<TableStyleInfo> styleInfoList);

        //relatedIdentities从大到小，最后是0
        TableStyleInfo GetTableStyleInfo(string tableName, string attributeName, List<int> relatedIdentities);

        TableStyleInfo GetTableStyleInfo(int id);

        Dictionary<string, List<TableStyleInfo>> GetTableStyleInfoWithItemsDictionary(string tableName, List<int> allRelatedIdentities);

        string GetValidateInfo(TableStyleInfo styleInfo);

        List<int> GetRelatedIdentities(int siteId);

        List<int> GetRelatedIdentities(ChannelInfo channelInfo);

        List<int> EmptyRelatedIdentities { get; }
    }

}
