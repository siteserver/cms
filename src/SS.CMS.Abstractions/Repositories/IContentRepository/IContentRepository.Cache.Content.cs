using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        Task<Content> GetContentInfoAsync(int contentId);
    }
}