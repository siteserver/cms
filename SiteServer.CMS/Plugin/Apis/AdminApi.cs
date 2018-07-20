using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Apis
{
    public class AdminApi : IAdminApi
    {
        private AdminApi() { }

        private static AdminApi _instance;
        public static AdminApi Instance => _instance ?? (_instance = new AdminApi());

        public bool IsAdminNameExists(string adminName)
        {
            return DataProvider.AdministratorDao.IsAdminNameExists(adminName);
        }

        public string AdminName
        {
            get
            {
                var request = new AuthRequest();
                return request.AdminName;
            }
        }

        public bool HasSystemPermissions(params string[] systemPermissions)
        {
            var request = new AuthRequest();
            return request.AdminPermissions.HasSystemPermissions(systemPermissions);
        }

        public bool HasSitePermissions(int siteId, params string[] sitePermissions)
        {
            var request = new AuthRequest();
            return request.AdminPermissions.HasSitePermissions(siteId, sitePermissions);
        }

        public bool HasChannelPermissions(int siteId, int channelId, params string[] channelPermissions)
        {
            var request = new AuthRequest();
            return request.AdminPermissions.HasChannelPermissions(siteId, channelId, channelPermissions);
        }
    }
}
