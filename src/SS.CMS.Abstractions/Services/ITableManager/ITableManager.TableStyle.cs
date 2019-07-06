using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface ITableManager
    {
        Task<List<TableStyleInfo>> GetStyleInfoListAsync(string tableName, List<int> relatedIdentities);

        Task<List<TableStyleInfo>> GetSiteStyleInfoListAsync(int siteId);

        Task<List<TableStyleInfo>> GetChannelStyleInfoListAsync(ChannelInfo channelInfo);

        Task<List<TableStyleInfo>> GetContentStyleInfoListAsync(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo);

        Task<List<TableStyleInfo>> GetUserStyleInfoListAsync();

        IDictionary<string, object> GetDefaultAttributes(List<TableStyleInfo> styleInfoList);

        //relatedIdentities从大到小，最后是0
        Task<TableStyleInfo> GetTableStyleInfoAsync(string tableName, string attributeName, List<int> relatedIdentities);

        Task<TableStyleInfo> GetTableStyleInfoAsync(int id);

        Task<Dictionary<string, List<TableStyleInfo>>> GetTableStyleInfoWithItemsDictionaryAsync(string tableName, List<int> allRelatedIdentities);

        string GetValidateInfo(TableStyleInfo styleInfo);

        List<int> GetRelatedIdentities(int siteId);

        List<int> GetRelatedIdentities(ChannelInfo channelInfo);

        List<int> EmptyRelatedIdentities { get; }


    }

}
