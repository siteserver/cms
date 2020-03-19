using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IContentRepository
    {
        Task<int> GetCountCheckingAsync(Site site);
    }
}