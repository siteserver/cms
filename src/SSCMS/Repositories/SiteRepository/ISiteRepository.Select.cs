using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface ISiteRepository
    {
        Task<Site> GetAsync(int siteId);
    }
}