using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Common
{
    public partial class Request
    {
        public bool IsUserLoggin { get; }

        public string UserToken
        {
            get
            {
                var accessToken = string.Empty;
                if (CookieManager.TryGet(CookieManager.UserToken, this, out var cookie))
                {
                    accessToken = cookie;
                }
                else if (HeaderManager.TryGet(HeaderManager.UserToken, this, out var header))
                {
                    accessToken = header;
                }
                else if (QueryManager.TryGet(QueryManager.UserToken, this, out var query))
                {
                    accessToken = query;
                }

                return StringUtils.IsEncrypted(accessToken) ? TranslateUtils.DecryptStringBySecretKey(accessToken) : accessToken;
            }
        }

        private PermissionsImpl _userPermissionsImpl;

        public PermissionsImpl UserPermissionsImpl
        {
            get
            {
                if (_userPermissionsImpl != null) return _userPermissionsImpl;

                if (UserInfo != null)
                {
                    var groupInfo = UserGroupManager.GetUserGroupInfo(UserInfo.GroupId);
                    if (groupInfo != null)
                    {
                        _adminInfo = AdminManager.GetAdminInfoByUserName(groupInfo.AdminName);
                    }
                }

                _userPermissionsImpl = new PermissionsImpl(AdminInfo);

                return _userPermissionsImpl;
            }
        }

        public IPermissions UserPermissions => UserPermissionsImpl;

        public int UserId => UserInfo?.Id ?? 0;

        public string UserName => UserInfo?.UserName ?? string.Empty;

        public IUserInfo UserInfo => _userInfo;
    }
}
