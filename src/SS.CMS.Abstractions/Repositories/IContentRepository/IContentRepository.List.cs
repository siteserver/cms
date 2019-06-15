using System.Collections.Generic;
using System.Collections.Specialized;
using SqlKata;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Repositories
{
    public partial interface IContentRepository
    {
        IList<int> GetContentIdList(int channelId);

        IList<int> GetContentIdList(int channelId, bool isPeriods, string dateFrom, string dateTo, bool? checkedState);

        IList<int> GetContentIdListCheckedByChannelId(int siteId, int channelId);

        IList<(int, int)> GetContentIdListByTrash(int siteId);

        List<int> GetContentIdListChecked(int channelId, TaxisType taxisType);

        IList<(int, int)> ApiGetContentIdListByChannelId(int siteId, int channelId, int top, int skip, string like, string orderBy, NameValueCollection queryString, out int totalCount);

        IList<int> GetIdListBySameTitle(int channelId, string title);

        IList<int> GetChannelIdListCheckedByLastEditDateHour(int siteId, int hour);

        List<int> GetCacheContentIdList(Query query, int offset, int limit);

        IList<string> GetValueListByStartString(int channelId, string name, string startString, int totalNum);

        List<ContentInfo> GetContentInfoList(Query query, int offset, int limit);

        List<ContentCountInfo> GetContentCountInfoList();
    }
}
