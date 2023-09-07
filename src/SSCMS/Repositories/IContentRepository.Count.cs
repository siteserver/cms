using System;
using System.Threading.Tasks;
using SqlKata;
using SSCMS.Models;
using SSCMS.Enums;

namespace SSCMS.Repositories
{
    public partial interface IContentRepository
    {
        Task<int> GetCountAsync(string tableName, Query query);

        Task<int> GetCountAsync(Site site, IChannelSummary channel);

        Task<int> GetCountOfCheckedAsync(Site site, IChannelSummary channel);

        Task<int> GetCountCheckedImageAsync(Site site, Channel channel);

        Task<int> GetCountOfCheckedImagesAsync(Site site, IChannelSummary channel);

        Task<int> GetCountOfContentUpdateAsync(string tableName, int siteId, int channelId, ScopeType scope,
            DateTime begin, DateTime end, int adminId);

        Task<int> GetCountOfContentAddAsync(string tableName, int siteId, int channelId, ScopeType scope,
            DateTime begin, DateTime end, int adminId, bool? checkedState);
        
        Task<int> GetCountCheckingAsync(Site site);
    }
}