using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Repositories
{
    public partial interface ISiteRepository
    {
        List<SiteInfo> GetSiteInfoList();

        SiteInfo GetSiteInfo(int siteId);

        SiteInfo GetSiteInfoBySiteName(string siteName);

        SiteInfo GetSiteInfoByIsRoot();

        SiteInfo GetSiteInfoByDirectory(string siteDir);

        List<int> GetSiteIdList();

        List<int> GetSiteIdListOrderByLevel();

        void GetAllParentSiteIdList(List<int> parentSiteIds, List<int> siteIdCollection, int siteId);

        bool IsExists(int siteId);

        Task<List<string>> GetSiteTableNamesAsync(IPluginManager pluginManager);

        Task<List<string>> GetAllTableNameListAsync(IPluginManager pluginManager);

        Task<List<string>> GetTableNameListAsync(IPluginManager pluginManager, SiteInfo siteInfo);

        int GetSiteLevel(int siteId);

        int GetParentSiteId(int siteId);

        string GetSiteName(SiteInfo siteInfo);

        int GetTableCount(string tableName);
    }
}
