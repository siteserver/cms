using System.Threading.Tasks;

namespace SSCMS.Abstractions
{
    public partial interface ISiteRepository
    {
        Task<Site> GetAsync(int siteId);
    }
}