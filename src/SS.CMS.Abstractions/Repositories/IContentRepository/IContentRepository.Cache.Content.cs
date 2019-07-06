using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        ContentInfo GetContentInfo(int contentId);
    }
}