using System.Collections.Generic;
using SS.CMS.Models;
using System.Threading.Tasks;

namespace SS.CMS.Repositories
{
    public partial interface ITagRepository
    {
        Task<IList<int>> GetContentIdListByTagCollectionAsync(List<string> tagCollection, int siteId);

        Task<IEnumerable<TagInfo>> GetTagInfoListAsync(int siteId, int contentId, bool isOrderByCount, int totalNum);
    }
}
