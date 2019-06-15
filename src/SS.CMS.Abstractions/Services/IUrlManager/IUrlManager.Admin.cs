using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IUrlManager
    {
        string AdminIndexUrl { get; }

        string AdminInstallUrl { get; }
        string AdminDashboardUrl { get; }
        string AdminErrorUrl { get; }
        string AdminLoginUrl { get; }
        string AdminSyncUrl { get; }

        string GetAdminUrl(string relatedUrl);

        string GetAdminIndexUrl(int? siteId, string pageUrl);

        string GetAdminContentsUrl(int siteId, int channelId);

        string GetAdminCreateStatusUrl(int siteId);
    }
}
