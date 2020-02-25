using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IContentRepository
    {
        Task<int> GetCountCheckingAsync(Site site);
    }
}