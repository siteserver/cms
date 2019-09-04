using System;
using System.Threading.Tasks;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IContentRepository
    {
        Task<int> GetMaxTaxisAsync(int channelId, bool isTop);

        Task<(int ChannelId, T Value)?> GetChanelIdAndValueAsync<T>(int contentId, string name);

        Task<T> GetValueAsync<T>(int contentId, string name);

        Task<int> GetTotalHitsAsync(int siteId);

        Task<int> GetFirstContentIdAsync(int siteId, int channelId);

        Task<int> GetContentIdAsync(int channelId, int taxis, bool isNextContent);

        Task<int> GetChannelIdAsync(int contentId);

        Task<int> GetContentIdAsync(int channelId, TaxisType taxisType);

        Task<int> GetTaxisToInsertAsync(int channelId, bool isTop);

        Task<int> GetSequenceAsync(int channelId, int contentId);

        Task<int> GetCountCheckedImageAsync(int siteId, int channelId);

        Task<int> GetCountOfContentAddAsync(int siteId, int channelId, ScopeType scope, DateTime begin, DateTime end, int userId, bool? checkedState);

        Task<int> GetCountOfContentUpdateAsync(int siteId, int channelId, ScopeType scope, DateTime begin, DateTime end, int userId);

        Task<Content> GetCacheContentInfoAsync(int contentId);
    }
}
