using System.Threading.Tasks;

namespace SS.CMS.Repositories
{
    public partial interface ISiteLogRepository
    {
        Task AddSiteLogAsync(int siteId, int channelId, int contentId, string ipAddress, string adminName, string action, string summary);
    }
}
