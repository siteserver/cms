using System.Collections.Generic;
using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IContentTagRepository
    {
        Task<IList<int>> GetContentIdListByTagCollectionAsync(List<string> tagCollection, int siteId);

        Task<IEnumerable<ContentTag>> GetTagInfoListAsync(int siteId, int contentId, bool isOrderByCount, int totalNum);
    }
}
