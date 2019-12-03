using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IContentRepository
    {
        Task DeleteAsync(int siteId, int contentId);
    }
}
