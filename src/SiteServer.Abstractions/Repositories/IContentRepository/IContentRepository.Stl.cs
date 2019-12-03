using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SqlKata;

namespace SiteServer.Abstractions
{
    public partial interface IContentRepository
    {
        Task<List<KeyValuePair<int, Content>>> GetContainerContentListCheckedAsync(List<int> channelIdList, int startNum, int totalNum, string order, Query query, NameValueCollection others);

        Task<List<KeyValuePair<int, Content>>> GetContainerContentListByContentNumAndWhereStringAsync(int totalNum, Query query, string order);

        Task<List<KeyValuePair<int, Content>>> GetContainerContentListByStartNumAsync(int startNum, int totalNum, Query query, string order);

        List<KeyValuePair<int, Content>> GetContainerContentListBySqlString(string sqlString, string orderString, int totalCount, int itemsPerPage, int currentPageIndex);
    }
}
