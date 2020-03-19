using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface ISiteRepository
    {
        Task<List<int>> GetLatestSiteIdListAsync(List<int> siteIdListLatestAccessed,
            List<int> siteIdListWithPermissions);
    }
}
