using System.Threading.Tasks;

namespace SSCMS.Abstractions
{
    public partial interface IContentRepository
    {
        Task<int> GetCountCheckingAsync(Site site);
    }
}