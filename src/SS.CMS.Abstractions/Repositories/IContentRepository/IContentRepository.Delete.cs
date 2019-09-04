using System.Threading.Tasks;

namespace SS.CMS.Repositories
{
    public partial interface IContentRepository
    {
        Task DeleteAsync(int siteId, int contentId);
    }
}
