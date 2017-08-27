using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Permissions;
using SiteServer.Plugin.Apis;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin.Apis
{
    public class AuthApi : IAuthApi
    {
        private readonly PluginMetadata _metadata;

        public AuthApi(PluginMetadata metadata)
        {
            _metadata = metadata;
        }

        public bool IsAuthorized()
        {
            var body = new RequestBody();
            return PermissionsManager.HasAdministratorPermissions(body.AdministratorName, _metadata.Id);
        }

        public bool IsAuthorized(int publishmentSystemId)
        {
            var body = new RequestBody();
            return PermissionsManager.HasAdministratorPermissions(body.AdministratorName, _metadata.Id + publishmentSystemId);
        }

        public string EncryptStringBySecretKey(string inputString)
        {
            return TranslateUtils.EncryptStringBySecretKey(inputString);
        }

        public string DecryptStringBySecretKey(string inputString)
        {
            return TranslateUtils.DecryptStringBySecretKey(inputString);
        }
    }
}
