using System;
using System.Threading.Tasks;

namespace SiteServer.Abstractions
{
    public partial interface ISiteLogRepository
    {
        Task AddSiteLogAsync(int siteId, int channelId, int contentId, Administrator adminInfo,
            string action, string summary);
    }
}
