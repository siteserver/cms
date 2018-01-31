using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Apis
{
    public class AdminApi : IAdminApi
    {
        private readonly IMetadata _metadata;

        public AdminApi(IMetadata metadata)
        {
            _metadata = metadata;
        }

        public bool IsAdminNameExists(string adminName)
        {
            return DataProvider.AdministratorDao.IsAdminNameExists(adminName);
        }

        public string AdminName
        {
            get
            {
                var request = new Request();
                return request.AdminName;
            }
        }

        public bool IsPluginAuthorized
        {
            get
            {
                var request = new Request();
                return PermissionsManager.HasAdministratorPermissions(request.AdminName, _metadata.Id);
            }
        }

        public bool IsSiteAuthorized(int siteId)
        {
            var request = new Request();
            return PermissionsManager.HasAdministratorPermissions(request.AdminName, _metadata.Id + siteId);
        }

        public bool HasSitePermissions(int siteId, params string[] sitePermissions)
        {
            var request = new Request();
            return AdminUtility.HasSitePermissions(request.AdminName, siteId, sitePermissions);
        }

        public bool HasChannelPermissions(int siteId, int channelId, params string[] channelPermissions)
        {
            var request = new Request();
            return AdminUtility.HasChannelPermissions(request.AdminName, siteId, channelId, channelPermissions);
        }
    }
}
