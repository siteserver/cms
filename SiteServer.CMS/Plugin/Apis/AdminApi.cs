using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.Plugin.Apis;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin.Apis
{
    public class AdminApi : IAdminApi
    {
        private readonly PluginMetadata _metadata;

        public AdminApi(PluginMetadata metadata)
        {
            _metadata = metadata;
        }

        public bool IsAdminNameExists(string adminName)
        {
            return BaiRongDataProvider.AdministratorDao.IsAdminNameExists(adminName);
        }

        public string AdminName
        {
            get
            {
                var body = new RequestBody();
                return body.AdminName;
            }
        }

        public bool IsPluginAuthorized
        {
            get
            {
                var body = new RequestBody();
                return PermissionsManager.HasAdministratorPermissions(body.AdminName, _metadata.Id);
            }
        }

        public bool IsSiteAuthorized(int publishmentSystemId)
        {
            var body = new RequestBody();
            return PermissionsManager.HasAdministratorPermissions(body.AdminName, _metadata.Id + publishmentSystemId);
        }

        public bool HasSitePermissions(int publishmentSystemId, params string[] sitePermissions)
        {
            var body = new RequestBody();
            return AdminUtility.HasSitePermissions(body.AdminName, publishmentSystemId, sitePermissions);
        }

        public bool HasChannelPermissions(int publishmentSystemId, int channelId, params string[] channelPermissions)
        {
            var body = new RequestBody();
            return AdminUtility.HasChannelPermissions(body.AdminName, publishmentSystemId, channelId, channelPermissions);
        }
    }
}
