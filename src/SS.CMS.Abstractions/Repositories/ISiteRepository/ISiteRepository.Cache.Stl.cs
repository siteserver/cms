using System.Threading.Tasks;

namespace SS.CMS.Repositories
{
    public partial interface ISiteRepository
    {
        Task<int> GetSiteIdByIsRootAsync();

        Task<int> GetSiteIdBySiteDirAsync(string siteDir);
    }
}
