using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface ISiteRepository
    {
        Task<List<Site>> GetSitesAsync();

        Task<int> GetParentSiteIdAsync(int siteId);

        Task<string> GetTableNameAsync(int siteId);

        Task<Site> GetSiteBySiteNameAsync(string siteName);

        Task<Site> GetSiteByIsRootAsync();

        Task<bool> IsRootExistsAsync();

        Task<Site> GetSiteByDirectoryAsync(string siteDir);

        Task<List<Cascade<int>>> GetSiteOptionsAsync(int parentId);

        Task<List<int>> GetSiteIdsAsync();

        Task<List<int>> GetSiteIdsOrderByLevelAsync();

        Task<List<int>> GetSiteIdsAsync(int parentId);

        Task<List<string>> GetSiteTableNamesAsync();

        Task<List<string>> GetAllTableNamesAsync();

        Task<List<string>> GetTableNamesAsync(Site site);

        Task<int> GetIdByIsRootAsync();

        Task<IList<string>> GetSiteDirsAsync(int parentId);

        Task<List<Select<int>>> GetSelectsAsync(List<int> includedSiteIds = null);

        Task<List<Site>> GetSitesWithChildrenAsync(int parentId, Func<Site, Task<object>> func = null);

        Task<List<Cascade<int>>> GetCascadeChildrenAsync(int parentId, Func<SiteSummary, Task<object>> func = null);

        Task<List<Cascade<int>>> GetCascadeChildrenAsync(int parentId, Func<SiteSummary, object> func);
    }
}