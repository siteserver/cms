using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Services;

namespace SS.CMS.Abstractions.Repositories
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

        List<string> GetSiteTableNames(IPluginManager pluginManager);

        List<string> GetAllTableNameList(IPluginManager pluginManager);

        List<string> GetTableNameList(IPluginManager pluginManager, SiteInfo siteInfo);

        int GetSiteLevel(int siteId);

        int GetParentSiteId(int siteId);

        string GetSiteName(IUrlManager urlManager, SiteInfo siteInfo);

        int GetTableCount(string tableName);

        string GetSourceName(int sourceId);
    }
}
