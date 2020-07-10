using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Repositories
{
    public partial interface ISiteRepository
    {
        Task<List<int>> GetLatestSiteIdsAsync(List<int> siteIdsLatestAccessed,
            List<int> siteIdsWithPermissions);
    }
}
