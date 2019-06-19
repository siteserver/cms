using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class UserManager
    {
        public bool IsSuperAdministrator()
        {
            return _context.User.IsInRole(AuthTypes.Roles.SuperAdministrator);
        }

        public bool IsSiteAdministrator(int siteId)
        {
            if (_context.User.IsInRole(AuthTypes.Roles.SuperAdministrator))
            {
                return true;
            }

            if (_context.User.IsInRole($"{siteId}:{AuthTypes.Roles.SiteAdministrator}"))
            {
                return true;
            }

            return false;
        }

        public IList<string> GetRoles()
        {
            var list = new List<string>();
            var claims = _context.User.FindAll(x => x.Type == AuthTypes.ClaimTypes.Role);
            foreach (var claim in claims)
            {
                if (!string.IsNullOrWhiteSpace(claim.Value) && !list.Contains(claim.Value))
                {
                    list.Add(claim.Value);
                }
            }
            return list;
        }

        public bool HasAppPermissions(params string[] permissions)
        {
            if (_context.User.IsInRole(AuthTypes.Roles.SuperAdministrator))
            {
                return true;
            }

            var appPermissions = _permissionRepository.GetAppPermissions(GetRoles());

            return permissions.Any(permission =>
            {
                return appPermissions.Contains(permission);
            });
        }

        public bool HasSitePermissions()
        {
            if (_context.User.IsInRole(AuthTypes.Roles.SuperAdministrator))
            {
                return true;
            }

            var siteIdList = _siteRepository.GetSiteIdList();
            return siteIdList.Any(HasSitePermissions);
        }

        public bool HasSitePermissions(int siteId)
        {
            if (_context.User.IsInRole(AuthTypes.Roles.SuperAdministrator))
            {
                return true;
            }

            if (_context.User.IsInRole($"{siteId}:{AuthTypes.Roles.SiteAdministrator}"))
            {
                return true;
            }

            var sitePermissions = _permissionRepository.GetSitePermissions(GetRoles(), siteId);

            return sitePermissions.Count > 0;
        }

        public bool HasSitePermissions(int siteId, params string[] permissions)
        {
            if (_context.User.IsInRole(AuthTypes.Roles.SuperAdministrator))
            {
                return true;
            }

            if (_context.User.IsInRole($"{AuthTypes.Roles.SiteAdministrator}:{siteId}"))
            {
                return true;
            }

            var sitePermissions = _permissionRepository.GetSitePermissions(GetRoles(), siteId);

            return permissions.Any(permission =>
            {
                return sitePermissions.Contains(permission);
            });
        }

        public bool HasChannelPermissions(int siteId, int channelId, params string[] permissions)
        {
            if (_context.User.IsInRole(AuthTypes.Roles.SuperAdministrator))
            {
                return true;
            }

            if (_context.User.IsInRole($"{siteId}:{AuthTypes.Roles.SiteAdministrator}"))
            {
                return true;
            }

            var channelPermissions = _permissionRepository.GetChannelPermissions(GetRoles(), siteId, channelId);

            return permissions.Any(permission =>
            {
                return channelPermissions.Contains(permission);
            });
        }

        public async Task<IList<int>> GetSiteIdsAsync()
        {
            var siteIdList = new List<int>();

            if (IsSuperAdministrator())
            {
                siteIdList = _siteRepository.GetSiteIdList();
            }
            else if (HasSitePermissions())
            {
                var adminInfo = await GetUserAsync();
                foreach (var siteId in TranslateUtils.StringCollectionToIntList(adminInfo.SiteIdCollection))
                {
                    if (!siteIdList.Contains(siteId))
                    {
                        siteIdList.Add(siteId);
                    }
                }
            }
            else
            {
                //var dict = WebsitePermissionDict;

                //foreach (var siteId in dict.Keys)
                //{
                //    if (!siteIdList.Contains(siteId))
                //    {
                //        siteIdList.Add(siteId);
                //    }
                //}
            }

            return siteIdList;
        }

        public int? GetOnlyAdminId(int siteId, int channelId)
        {
            if (!_configRepository.Instance.IsViewContentOnlySelf
                || IsSuperAdministrator()
                || IsSiteAdministrator(siteId)
                || HasChannelPermissions(siteId, channelId, AuthTypes.ChannelPermissions.ContentCheck))
            {
                return null;
            }
            return GetUserId();
        }
    }
}
