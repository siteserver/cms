using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        Task<ContentInfo> GetContentInfoAsync(int contentId);
    }
}