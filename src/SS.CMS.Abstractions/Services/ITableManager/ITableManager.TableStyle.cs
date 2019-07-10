using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface ITableManager
    {
        Task<List<TableStyle>> GetStyleInfoListAsync(string tableName, List<int> relatedIdentities);

        Task<List<TableStyle>> GetSiteStyleInfoListAsync(int siteId);

        Task<List<TableStyle>> GetChannelStyleInfoListAsync(Channel channelInfo);

        Task<List<TableStyle>> GetContentStyleInfoListAsync(IPluginManager pluginManager, Site siteInfo, Channel channelInfo);

        Task<List<TableStyle>> GetUserStyleInfoListAsync();

        IDictionary<string, object> GetDefaultAttributes(List<TableStyle> styleInfoList);

        //relatedIdentities从大到小，最后是0
        Task<TableStyle> GetTableStyleInfoAsync(string tableName, string attributeName, List<int> relatedIdentities);

        Task<TableStyle> GetTableStyleInfoAsync(int id);

        Task<Dictionary<string, List<TableStyle>>> GetTableStyleInfoWithItemsDictionaryAsync(string tableName, List<int> allRelatedIdentities);

        string GetValidateInfo(TableStyle styleInfo);

        List<int> GetRelatedIdentities(int siteId);

        List<int> GetRelatedIdentities(Channel channelInfo);

        List<int> EmptyRelatedIdentities { get; }


    }

}
