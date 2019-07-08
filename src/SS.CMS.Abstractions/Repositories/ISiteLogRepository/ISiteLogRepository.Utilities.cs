using System.Threading.Tasks;

namespace SS.CMS.Repositories
{
    public partial interface ISiteLogRepository
    {
        Task AddSiteLogAsync(int siteId, int channelId, int contentId, string ipAddress, int userId, string action, string summary);
    }
}
