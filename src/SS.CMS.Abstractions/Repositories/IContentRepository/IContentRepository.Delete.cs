using System.Threading.Tasks;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        Task DeleteAsync(int siteId, int contentId);
    }
}
