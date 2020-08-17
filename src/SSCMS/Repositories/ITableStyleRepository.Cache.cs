using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface ITableStyleRepository
    {
        Task<List<TableStyle>> GetTableStylesAsync(string tableName, List<int> relatedIdentities, List<string> excludeAttributeNames = null);

        Task<List<TableStyle>> GetSiteStylesAsync(int siteId);

        Task<List<TableStyle>> GetChannelStylesAsync(Channel channel);

        Task<List<TableStyle>> GetContentStylesAsync(Site site, Channel channel);

        Task<List<TableStyle>> GetUserStylesAsync();

        Task<TableStyle> GetTableStyleAsync(string tableName, string attributeName, List<int> relatedIdentities);

        Task<bool> IsExistsAsync(int relatedIdentity, string tableName, string attributeName);

        Task<Dictionary<string, List<TableStyle>>> GetTableStyleWithItemsDictionaryAsync(string tableName,
            List<int> allRelatedIdentities);

        List<int> GetRelatedIdentities(int relatedIdentity);

        List<int> GetRelatedIdentities(Channel channel);

        List<int> EmptyRelatedIdentities { get; }
    }
}
