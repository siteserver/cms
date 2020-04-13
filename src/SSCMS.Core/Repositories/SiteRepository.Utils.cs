using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Core.Repositories
{
    public partial class SiteRepository
    {
        public async Task<List<int>> GetLatestSiteIdListAsync(List<int> siteIdListLatestAccessed, List<int> siteIdListWithPermissions)
        {
            var siteIdList = new List<int>();

            foreach (var siteId in siteIdListLatestAccessed)
            {
                if (!siteIdList.Contains(siteId) && siteIdListWithPermissions.Contains(siteId))
                {
                    siteIdList.Add(siteId);
                }
            }

            var siteIdListOrderByLevel = await GetSiteIdListOrderByLevelAsync();
            foreach (var siteId in siteIdListOrderByLevel)
            {
                if (!siteIdList.Contains(siteId) && siteIdListWithPermissions.Contains(siteId))
                {
                    siteIdList.Add(siteId);
                }
            }

            return siteIdList;
        }
    }
}
