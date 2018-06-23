using SiteServer.CMS.Core;
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
                var request = new AuthRequest();
                return request.AdminName;
            }
        }

        public bool IsPluginAuthorized
        {
            get
            {
                var request = new AuthRequest();
                return request.AdminPermissions.HasAdministratorPermissions(_metadata.Id);
            }
        }

        public bool IsSiteAuthorized(int siteId)
        {
            var request = new AuthRequest();
            return request.AdminPermissions.HasSitePermissions(siteId, _metadata.Id);
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
