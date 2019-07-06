using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SqlKata;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        Task<List<KeyValuePair<int, ContentInfo>>> GetContainerContentListCheckedAsync(List<int> channelIdList, int startNum, int totalNum, string order, Query query, NameValueCollection others);

        List<KeyValuePair<int, ContentInfo>> GetContainerContentListByContentNumAndWhereString(int totalNum, Query query, string order);

        List<KeyValuePair<int, ContentInfo>> GetContainerContentListByStartNum(int startNum, int totalNum, Query query, string order);

        List<KeyValuePair<int, ContentInfo>> GetContainerContentListBySqlString(string sqlString, string orderString, int totalCount, int itemsPerPage, int currentPageIndex);
    }
}
