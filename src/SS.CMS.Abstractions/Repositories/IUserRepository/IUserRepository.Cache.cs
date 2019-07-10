using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IUserRepository
    {
        Task RemoveCacheAsync(User userInfo);

        List<int> GetLatestTop10SiteIdList(List<int> siteIdListLatestAccessed, List<int> siteIdListOrderByLevel, List<int> siteIdListWithPermissions);

        Task<string> GetDisplayNameAsync(string userName);
    }
}