using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface IContentRepository
    {
        Task<int> GetCountCheckingAsync(Site site);
    }
}