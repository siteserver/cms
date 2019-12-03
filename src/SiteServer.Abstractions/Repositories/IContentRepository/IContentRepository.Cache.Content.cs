using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IContentRepository
    {
        Task<Content> GetContentInfoAsync(int contentId);
    }
}