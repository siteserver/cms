using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Dto;

namespace SSCMS
{
    public partial interface ISiteRepository
    {
        Task<List<Site>> GetSiteListAsync();

        Task<int> GetParentSiteIdAsync(int siteId);

        Task<string> GetTableNameAsync(int siteId);

        Task<Site> GetSiteBySiteNameAsync(string siteName);

        Task<Site> GetSiteByIsRootAsync();

        Task<bool> IsRootExistsAsync();

        Task<Site> GetSiteByDirectoryAsync(string siteDir);

        Task<List<Cascade<int>>> GetSiteOptionsAsync(int parentId);

        Task<List<int>> GetSiteIdListAsync();

        Task<List<int>> GetSiteIdListOrderByLevelAsync();

        Task<List<int>> GetSiteIdListAsync(int parentId);

        Task<List<string>> GetSiteTableNamesAsync(IPluginManager pluginManager);

        Task<List<string>> GetAllTableNamesAsync(IPluginManager pluginManager);

        Task<List<string>> GetTableNamesAsync(IPluginManager pluginManager, Site site);

        Task<int> GetIdByIsRootAsync();

        Task<IList<string>> GetSiteDirListAsync(int parentId);

        Task<List<Select<int>>> GetSelectsAsync(List<int> includedSiteIds = null);

        Task<List<Site>> GetSitesWithChildrenAsync(int parentId, Func<Site, Task<object>> func = null);

        Task<List<Cascade<int>>> GetCascadeChildrenAsync(int parentId, Func<SiteSummary, Task<object>> func = null);
    }
}