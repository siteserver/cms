using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Plugin.Impl;
using SS.CMS.Core.Settings;
using SS.CMS.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Api.Common
{
    public partial class Request
    {
        public string AdminToken
        {
            get
            {
                var accessToken = string.Empty;
                if (CookieManager.TryGet(CookieManager.AdminToken, this, out var cookie))
                {
                    accessToken = cookie;
                }
                else if (HeaderManager.TryGet(HeaderManager.AdminToken, this, out var header))
                {
                    accessToken = header;
                }
                else if (QueryManager.TryGet(QueryManager.AdminToken, this, out var query))
                {
                    accessToken = query;
                }

                return StringUtils.IsEncrypted(accessToken) ? AppContext.Decrypt(accessToken) : accessToken;
            }
        }

        public bool IsAdminLoggin { get; private set; }

        private PermissionsImpl _adminPermissionsImpl;

        public PermissionsImpl AdminPermissionsImpl
        {
            get
            {
                if (_adminPermissionsImpl != null) return _adminPermissionsImpl;

                _adminPermissionsImpl = new PermissionsImpl(AdminInfo);

                return _adminPermissionsImpl;
            }
        }

        public IPermissions AdminPermissions => AdminPermissionsImpl;

        public int AdminId => AdminInfo?.Id ?? 0;

        public string AdminName
        {
            get
            {
                if (AdminInfo != null)
                {
                    return AdminInfo.UserName;
                }

                if (UserInfo != null)
                {
                    var groupInfo = UserGroupManager.GetUserGroupInfo(UserInfo.GroupId);
                    if (groupInfo != null)
                    {
                        return groupInfo.AdminName;
                    }
                }

                return string.Empty;
            }
        }

        public IAdministratorInfo AdminInfo => _adminInfo;

        
    }
}
