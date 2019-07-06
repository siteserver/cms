using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Repositories
{
    public partial interface ISiteRepository
    {
        Task<List<SiteInfo>> GetSiteInfoListAsync();

        Task<SiteInfo> GetSiteInfoAsync(int siteId);

        Task<SiteInfo> GetSiteInfoBySiteNameAsync(string siteName);

        Task<SiteInfo> GetSiteInfoByIsRootAsync();

        Task<SiteInfo> GetSiteInfoByDirectoryAsync(string siteDir);

        Task<List<int>> GetSiteIdListAsync();

        Task<List<int>> GetSiteIdListOrderByLevelAsync();

        Task GetAllParentSiteIdListAsync(List<int> parentSiteIds, List<int> siteIdCollection, int siteId);

        Task<bool> IsExistsAsync(int siteId);

        Task<List<string>> GetSiteTableNamesAsync(IPluginManager pluginManager);

        Task<List<string>> GetAllTableNameListAsync(IPluginManager pluginManager);

        Task<List<string>> GetTableNameListAsync(IPluginManager pluginManager, SiteInfo siteInfo);

        Task<int> GetSiteLevelAsync(int siteId);

        Task<int> GetParentSiteIdAsync(int siteId);

        Task<string> GetSiteNameAsync(SiteInfo siteInfo);

        Task<bool> IsSiteTableAsync(string tableName);
    }
}
