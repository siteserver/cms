using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly Repository<PermissionInfo> _repository;
        private readonly IRoleRepository _roleRepository;

        public PermissionRepository(ISettingsManager settingsManager, IRoleRepository roleRepository)
        {
            _repository = new Repository<PermissionInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _roleRepository = roleRepository;
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string RoleName = nameof(PermissionInfo.RoleName);
            public const string AppPermissions = nameof(PermissionInfo.AppPermissions);
            public const string SiteId = nameof(PermissionInfo.SiteId);
            public const string SitePermissions = nameof(PermissionInfo.SitePermissions);
            public const string ChannelId = nameof(PermissionInfo.ChannelId);
            public const string ChannelPermissions = nameof(PermissionInfo.ChannelPermissions);
        }

        public async Task<int> InsertAsync(PermissionInfo permissionsInfo)
        {
            if (await IsExistsAsync(permissionsInfo.RoleName, permissionsInfo.SiteId, permissionsInfo.ChannelId))
            {
                await DeleteAsync(permissionsInfo.RoleName, permissionsInfo.SiteId, permissionsInfo.ChannelId);
            }
            return await _repository.InsertAsync(permissionsInfo);
        }

        public async Task<bool> DeleteAsync(string roleName)
        {
            return await _repository.DeleteAsync(Q
                .Where(Attr.RoleName, roleName)
            ) > 0;
        }

        private async Task DeleteAsync(string roleName, int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(Attr.RoleName, roleName)
                .Where(Attr.SiteId, siteId)
            );
        }

        private async Task DeleteAsync(string roleName, int siteId, int channelId)
        {
            await _repository.DeleteAsync(Q
                .Where(Attr.RoleName, roleName)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
            );
        }

        private async Task<bool> IsExistsAsync(string roleName, int siteId, int channelId)
        {
            return await _repository.ExistsAsync(Q
                .Where(Attr.RoleName, roleName)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
            );
        }

        private async Task<IEnumerable<PermissionInfo>> GetRolePermissionsInfoListAsync(string roleName)
        {
            return await _repository.GetAllAsync(Q.Where(Attr.RoleName, roleName));
        }

        public async Task<List<string>> GetAppPermissionsAsync(IEnumerable<string> roles)
        {
            var list = new List<string>();
            if (roles == null) return list;

            foreach (var roleName in roles)
            {
                var rolePermissionsInfoList = await GetRolePermissionsInfoListAsync(roleName);
                foreach (var rolePermissionsInfo in rolePermissionsInfoList)
                {
                    if (rolePermissionsInfo.SiteId > 0 || rolePermissionsInfo.ChannelId > 0) continue;

                    var permissionList = TranslateUtils.StringCollectionToStringList(rolePermissionsInfo.AppPermissions);
                    foreach (var permission in permissionList)
                    {
                        if (!string.IsNullOrWhiteSpace(permission) && !list.Contains(permission)) list.Add(permission);
                    }
                }
            }

            return list;
        }

        public async Task<List<string>> GetSitePermissionsAsync(IEnumerable<string> roles, int siteId)
        {
            var permissions = new List<string>();
            if (roles == null) return permissions;

            foreach (var roleName in roles)
            {
                var rolePermissionsInfoList = await GetRolePermissionsInfoListAsync(roleName);
                foreach (var rolePermissionsInfo in rolePermissionsInfoList)
                {
                    if (rolePermissionsInfo.SiteId != siteId || rolePermissionsInfo.ChannelId > 0) continue;

                    var permissionList = TranslateUtils.StringCollectionToStringList(rolePermissionsInfo.SitePermissions);
                    foreach (var permission in permissionList)
                    {
                        if (!string.IsNullOrWhiteSpace(permission) && !permissions.Contains(permission)) permissions.Add(permission);
                    }
                }
            }

            return permissions;
        }

        public async Task<List<string>> GetChannelPermissionsAsync(IEnumerable<string> roles, int siteId, int channelId)
        {
            var permissions = new List<string>();
            if (roles == null) return permissions;

            foreach (var roleName in roles)
            {
                var rolePermissionsInfoList = await GetRolePermissionsInfoListAsync(roleName);
                foreach (var rolePermissionsInfo in rolePermissionsInfoList)
                {
                    if (rolePermissionsInfo.SiteId != siteId || rolePermissionsInfo.ChannelId != channelId) continue;

                    var permissionList = TranslateUtils.StringCollectionToStringList(rolePermissionsInfo.ChannelPermissions);
                    foreach (var permission in permissionList)
                    {
                        if (!string.IsNullOrWhiteSpace(permission) && !permissions.Contains(permission)) permissions.Add(permission);
                    }
                }
            }

            return permissions;
        }
    }
}
