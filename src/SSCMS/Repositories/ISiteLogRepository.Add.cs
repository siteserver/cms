using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface ISiteLogRepository
    {
        Task AddSiteLogAsync(int siteId, int channelId, int contentId, Administrator adminInfo, string ipAddress,
            string action, string summary);
    }
}
