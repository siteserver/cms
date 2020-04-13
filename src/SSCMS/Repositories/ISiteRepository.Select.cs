using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface ISiteRepository
    {
        Task<Site> GetAsync(int siteId);
    }
}