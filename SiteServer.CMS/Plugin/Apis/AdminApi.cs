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

        public bool IsSiteAuthorized(int publishmentSystemId)
        {
            var request = new RequestContext();
            return PermissionsManager.HasAdministratorPermissions(request.AdminName, _metadata.Id + publishmentSystemId);
        }

        public bool HasSitePermissions(int publishmentSystemId, params string[] sitePermissions)
        {
            var request = new RequestContext();
            return AdminUtility.HasSitePermissions(request.AdminName, publishmentSystemId, sitePermissions);
        }

        public bool HasChannelPermissions(int publishmentSystemId, int channelId, params string[] channelPermissions)
        {
            var request = new RequestContext();
            return AdminUtility.HasChannelPermissions(request.AdminName, publishmentSystemId, channelId, channelPermissions);
        }
    }
}
