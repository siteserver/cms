using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class UserManager
    {
        public bool IsAdministrator()
        {
            return _context.User.IsInRole(AuthTypes.Roles.Administrator);
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

        public bool IsSuperAdministrator()
        {
            return _context.User.IsInRole(AuthTypes.Roles.SuperAdministrator);
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

        public async Task<bool> HasAppPermissionsAsync(params string[] permissions)
        {
            if (_context.User.IsInRole(AuthTypes.Roles.SuperAdministrator))
            {
                return true;
            }

            var appPermissions = await _permissionRepository.GetAppPermissionsAsync(GetRoles());

            return permissions.Any(permission =>
            {
                return appPermissions.Contains(permission);
            });
        }

        public async Task<bool> HasSitePermissionsAsync()
        {
            if (_context.User.IsInRole(AuthTypes.Roles.SuperAdministrator))
            {
                return true;
            }

            var siteIdList = await _siteRepository.GetSiteIdListAsync();
            foreach (var siteId in siteIdList)
            {
                if (await HasSitePermissionsAsync(siteId))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> HasSitePermissionsAsync(int siteId)
        {
            if (_context.User.IsInRole(AuthTypes.Roles.SuperAdministrator))
            {
                return true;
            }

            if (_context.User.IsInRole($"{siteId}:{AuthTypes.Roles.SiteAdministrator}"))
            {
                return true;
            }

            var sitePermissions = await _permissionRepository.GetSitePermissionsAsync(GetRoles(), siteId);

            return sitePermissions.Count > 0;
        }

        public async Task<bool> HasSitePermissionsAsync(int siteId, params string[] permissions)
        {
            if (_context.User.IsInRole(AuthTypes.Roles.SuperAdministrator))
            {
                return true;
            }

            if (_context.User.IsInRole($"{AuthTypes.Roles.SiteAdministrator}:{siteId}"))
            {
                return true;
            }

            var sitePermissions = await _permissionRepository.GetSitePermissionsAsync(GetRoles(), siteId);

            return permissions.Any(permission =>
            {
                return sitePermissions.Contains(permission);
            });
        }

        public async Task<bool> HasChannelPermissionsAsync(int siteId, int channelId, params string[] permissions)
        {
            if (_context.User.IsInRole(AuthTypes.Roles.SuperAdministrator))
            {
                return true;
            }

            if (_context.User.IsInRole($"{siteId}:{AuthTypes.Roles.SiteAdministrator}"))
            {
                return true;
            }

            var channelPermissions = await _permissionRepository.GetChannelPermissionsAsync(GetRoles(), siteId, channelId);

            return permissions.Any(permission =>
            {
                return channelPermissions.Contains(permission);
            });
        }

        public async Task<IList<int>> GetSiteIdsAsync()
        {
            var siteIdList = new List<int>();
            var allSiteIdList = await _siteRepository.GetSiteIdListAsync();

            if (IsSuperAdministrator())
            {
                siteIdList.AddRange(allSiteIdList);
            }
            else if (await HasSitePermissionsAsync())
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

        public async Task<int?> GetOnlyAdminIdAsync(int siteId, int channelId)
        {
            var configInfo = await _configRepository.GetConfigInfoAsync();

            if (!configInfo.IsViewContentOnlySelf
                || IsSuperAdministrator()
                || IsSiteAdministrator(siteId)
                || await HasChannelPermissionsAsync(siteId, channelId, AuthTypes.ChannelPermissions.ContentCheck))
            {
                return null;
            }
            return GetUserId();
        }
    }
}
