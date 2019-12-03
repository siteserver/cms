using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public interface ISiteRepository : IRepository
    {
        Task<int> InsertAsync(Site siteInfo);

        Task<bool> DeleteAsync(int siteId);

        Task<bool> UpdateAsync(Site siteInfo);

        Task UpdateTableNameAsync(int siteId, string tableName);

        Task<List<string>> GetSiteDirListAsync(int parentId);

        Task<List<KeyValuePair<int, Site>>> GetContainerSiteListAsync(string siteName, string siteDir, int startNum, int totalNum, ScopeType scopeType, string orderByString);

        Task<int> GetTableCountAsync(string tableName);

        Task<IEnumerable<int>> GetIdListAsync();

        Task<IList<Site>> GetSiteListAsync();

        Task<IList<Site>> GetSiteListAsync(int parentId);

        Task<Site> GetSiteAsync(int siteId);

        Task<Site> GetSiteBySiteNameAsync(string siteName);

        Task<Site> GetSiteByIsRootAsync();

        Task<int> GetIdByIsRootAsync();

        Task<Site> GetSiteBySiteDirAsync(string siteDir);

        Task<int> GetIdBySiteDirAsync(string siteDir);

        Task<List<int>> GetIdListOrderByLevelAsync();

        Task GetAllParentSiteIdListAsync(List<int> parentSiteIds, List<int> siteIdCollection, int siteId);

        Task<bool> IsExistsAsync(int siteId);

        Task<List<string>> GetSiteTableNamesAsync(IPluginManager pluginManager);

        Task<List<string>> GetAllTableNameListAsync(IPluginManager pluginManager);

        Task<List<string>> GetTableNameListAsync(IPluginManager pluginManager, Site siteInfo);

        Task<int> GetSiteLevelAsync(int siteId);

        Task<int> GetParentSiteIdAsync(int siteId);

        Task<string> GetSiteNameAsync(Site siteInfo);
    }
}