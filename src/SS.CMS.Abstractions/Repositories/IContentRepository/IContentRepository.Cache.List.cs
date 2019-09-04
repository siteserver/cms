using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IContentRepository
    {
        Task<IEnumerable<int>> GetContentIdListAsync(Site siteInfo, Channel channelInfo, int? onlyUserId, int offset, int limit);
    }
}