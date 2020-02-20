using System.Threading.Tasks;

namespace SiteServer.Abstractions
{
    public partial interface IContentRepository
    {
        Task<int> GetCountCheckingAsync(Site site);
    }
}