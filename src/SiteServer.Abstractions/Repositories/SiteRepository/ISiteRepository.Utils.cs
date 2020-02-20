using System.Collections.Generic;
using System.Threading.Tasks;

namespace SiteServer.Abstractions
{
    public partial interface ISiteRepository
    {
        Task<List<int>> GetLatestSiteIdListAsync(List<int> siteIdListLatestAccessed,
            List<int> siteIdListWithPermissions);
    }
}
