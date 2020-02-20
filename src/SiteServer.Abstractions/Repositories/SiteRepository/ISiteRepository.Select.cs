using System.Threading.Tasks;

namespace SiteServer.Abstractions
{
    public partial interface ISiteRepository
    {
        Task<Site> GetAsync(int siteId);
    }
}