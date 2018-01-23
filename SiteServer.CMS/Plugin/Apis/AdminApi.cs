using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Plugin.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Apis;

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
                var request = new RequestContext();
                return request.AdminName;
            }
        }

        public bool IsPluginAuthorized
        {
            get
            {
                var request = new RequestContext();
                return PermissionsManager.HasAdministratorPermissions(request.AdminName, _metadata.Id);
            }
        }

        public bool IsSiteAuthorized(int siteId)
        {
            var request = new RequestContext();
            return PermissionsManager.HasAdministratorPermissions(request.AdminName, _metadata.Id + siteId);
        }

        public bool HasSitePermissions(int siteId, params string[] sitePermissions)
        {
            var request = new RequestContext();
            return AdminUtility.HasSitePermissions(request.AdminName, siteId, sitePermissions);
        }

        public bool HasChannelPermissions(int siteId, int channelId, params string[] channelPermissions)
        {
            var request = new RequestContext();
            return AdminUtility.HasChannelPermissions(request.AdminName, siteId, channelId, channelPermissions);
        }
    }
}
