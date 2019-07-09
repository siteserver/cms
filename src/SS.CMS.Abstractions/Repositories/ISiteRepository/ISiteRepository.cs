using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Repositories
{
    public interface ISiteRepository : IRepository
    {
        Task<int> InsertAsync(SiteInfo siteInfo);

        Task<bool> DeleteAsync(int siteId);

        Task<bool> UpdateAsync(SiteInfo siteInfo);

        Task UpdateTableNameAsync(int siteId, string tableName);

        Task UpdateParentIdToZeroAsync(int parentId);

        Task<IEnumerable<string>> GetLowerSiteDirListThatNotIsRootAsync();

        Task<IEnumerable<string>> GetLowerSiteDirListAsync(int parentId);

        Task<List<KeyValuePair<int, SiteInfo>>> GetContainerSiteListAsync(string siteName, string siteDir, int startNum, int totalNum, ScopeType scopeType, string orderByString);

        Task<int> GetTableCountAsync(string tableName);

        Task<IEnumerable<int>> GetSiteIdListAsync();

        Task<List<SiteInfo>> GetSiteInfoListAsync();

        Task<SiteInfo> GetSiteInfoAsync(int siteId);

        Task<SiteInfo> GetSiteInfoBySiteNameAsync(string siteName);

        Task<SiteInfo> GetSiteInfoByIsRootAsync();

        Task<int> GetSiteIdByIsRootAsync();

        Task<SiteInfo> GetSiteInfoBySiteDirAsync(string siteDir);

        Task<int> GetSiteIdBySiteDirAsync(string siteDir);

        Task<List<int>> GetSiteIdListOrderByLevelAsync();

        Task GetAllParentSiteIdListAsync(List<int> parentSiteIds, List<int> siteIdCollection, int siteId);

        Task<bool> IsExistsAsync(int siteId);

        Task<List<string>> GetSiteTableNamesAsync(IPluginManager pluginManager);

        Task<List<string>> GetAllTableNameListAsync(IPluginManager pluginManager);

        Task<List<string>> GetTableNameListAsync(IPluginManager pluginManager, SiteInfo siteInfo);

        Task<int> GetSiteLevelAsync(int siteId);

        Task<int> GetParentSiteIdAsync(int siteId);

        Task<string> GetSiteNameAsync(SiteInfo siteInfo);
    }
}