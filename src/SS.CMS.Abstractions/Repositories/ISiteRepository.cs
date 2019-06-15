using System.Collections.Generic;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public partial interface ISiteRepository : IRepository
    {
        int Insert(SiteInfo siteInfo);

        bool Delete(int siteId);

        bool Update(SiteInfo siteInfo);

        void UpdateTableName(int siteId, string tableName);

        void UpdateParentIdToZero(int parentId);

        IList<string> GetLowerSiteDirListThatNotIsRoot();

        int GetIdByIsRoot();

        int GetIdBySiteDir(string siteDir);

        /// <summary>
        /// 得到所有系统文件夹的列表，以小写表示。
        /// </summary>
        List<string> GetLowerSiteDirList(int parentId);

        List<KeyValuePair<int, SiteInfo>> GetContainerSiteList(string siteName, string siteDir, int startNum, int totalNum, ScopeType scopeType, string orderByString);
    }
}