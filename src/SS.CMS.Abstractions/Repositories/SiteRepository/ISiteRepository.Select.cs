using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface ISiteRepository
    {
        Task<Site> GetAsync(int siteId);
    }
}