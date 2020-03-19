using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface ITableStyleRepository
    {
        Task<List<TableStyle>> GetStyleListAsync(string tableName, List<int> relatedIdentities);

        Task<List<TableStyle>> GetSiteStyleListAsync(int siteId);

        Task<List<TableStyle>> GetChannelStyleListAsync(Channel channel);

        Task<List<TableStyle>> GetContentStyleListAsync(Channel channel, string tableName);

        Task<List<TableStyle>> GetUserStyleListAsync();

        Task<TableStyle> GetTableStyleAsync(string tableName, string attributeName, List<int> relatedIdentities);

        Task<bool> IsExistsAsync(int relatedIdentity, string tableName, string attributeName);

        Task<Dictionary<string, List<TableStyle>>> GetTableStyleWithItemsDictionaryAsync(string tableName,
            List<int> allRelatedIdentities);

        List<int> GetRelatedIdentities(int relatedIdentity);

        List<int> GetRelatedIdentities(Channel channel);

        List<int> EmptyRelatedIdentities { get; }
    }
}
