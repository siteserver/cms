using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SqlKata;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        Task<IEnumerable<int>> GetContentIdListAsync(int channelId);

        Task<IEnumerable<int>> GetContentIdListAsync(int channelId, bool isPeriods, string dateFrom, string dateTo, bool? checkedState);

        Task<IEnumerable<int>> GetContentIdListCheckedByChannelIdAsync(int siteId, int channelId);

        Task<IEnumerable<(int, int)>> GetContentIdListByTrashAsync(int siteId);

        Task<IEnumerable<int>> GetContentIdListCheckedAsync(int channelId, TaxisType taxisType);

        //IList<(int, int)> ApiGetContentIdListByChannelId(int siteId, int channelId, int top, int skip, string like, string orderBy, NameValueCollection queryString, out int totalCount);

        Task<IEnumerable<int>> GetIdListBySameTitleAsync(int channelId, string title);

        Task<IEnumerable<int>> GetChannelIdListCheckedByLastEditDateHourAsync(int siteId, int hour);

        Task<IEnumerable<int>> GetCacheContentIdListAsync(Query query, int offset, int limit);

        Task<IEnumerable<string>> GetValueListByStartStringAsync(int channelId, string name, string startString, int totalNum);

        Task<IEnumerable<Content>> GetContentInfoListAsync(Query query, int offset, int limit);

        Task<IEnumerable<ContentCount>> GetContentCountInfoListAsync();
    }
}
