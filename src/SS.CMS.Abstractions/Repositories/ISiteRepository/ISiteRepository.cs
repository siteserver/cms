using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ISiteRepository : IRepository
    {
        Task<int> InsertAsync(SiteInfo siteInfo);

        Task<bool> DeleteAsync(int siteId);

        Task<bool> UpdateAsync(SiteInfo siteInfo);

        Task UpdateTableNameAsync(int siteId, string tableName);

        Task UpdateParentIdToZeroAsync(int parentId);

        IList<string> GetLowerSiteDirListThatNotIsRoot();

        List<string> GetLowerSiteDirList(int parentId);

        Task<List<KeyValuePair<int, SiteInfo>>> GetContainerSiteListAsync(string siteName, string siteDir, int startNum, int totalNum, ScopeType scopeType, string orderByString);

        int GetTableCount(string tableName);
    }
}