using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Repositories
{
    public partial interface ISiteRepository : IRepository
    {
        Task<int> InsertSiteAsync(IPathManager pathManager, Channel channel, Site site, int adminId);

        Task<int> InsertAsync(Site site);

        Task DeleteAsync(int siteId);

        Task UpdateAsync(Site site);

        Task UpdateTableNameAsync(int siteId, string tableName);

        Task UpdateParentIdToZeroAsync(int parentId);

        Task<List<KeyValuePair<int, Site>>> ParserGetSitesAsync(string siteName, string siteDir, int startNum,
            int totalNum, ScopeType scopeType, TaxisType taxisType);
    }
}
