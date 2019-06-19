using System;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        int GetMaxTaxis(int channelId, bool isTop);

        bool GetChanelIdAndValue<T>(int contentId, string name, out int channelId, out T value);

        T GetValue<T>(int contentId, string name);

        Tuple<int, T> GetValueWithChannelId<T>(int contentId, string name);

        int GetTotalHits(int siteId);

        int GetFirstContentId(int siteId, int channelId);

        int GetContentId(int channelId, int taxis, bool isNextContent);

        int GetChannelId(int contentId);

        int GetContentId(int channelId, TaxisType taxisType);

        int GetTaxisToInsert(int channelId, bool isTop);

        int GetSequence(int channelId, int contentId);

        int GetCountCheckedImage(int siteId, int channelId);

        int GetCountOfContentAdd(int siteId, int channelId, ScopeType scope, DateTime begin, DateTime end, string userName, bool? checkedState);

        int GetCountOfContentUpdate(int siteId, int channelId, ScopeType scope, DateTime begin, DateTime end, string userName);

        ContentInfo GetCacheContentInfo(int contentId);
    }
}
