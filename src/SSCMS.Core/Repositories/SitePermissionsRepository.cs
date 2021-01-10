using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Services;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class SitePermissionsRepository : ISitePermissionsRepository
    {
        private readonly Repository<SitePermissions> _repository;

        public SitePermissionsRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<SitePermissions>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(SitePermissions permissions)
        {
            if (await Exists(permissions.RoleName, permissions.SiteId))
            {
                await DeleteAsync(permissions.RoleName, permissions.SiteId);
            }
            await _repository.InsertAsync(permissions);
        }

        public async Task DeleteAsync(string roleName)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(SitePermissions.RoleName), roleName)
            );
        }

        public async Task DeleteAsync(string roleName, int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(SitePermissions.RoleName), roleName)
                .Where(nameof(SitePermissions.SiteId), siteId)
            );
        }

        public async Task<List<SitePermissions>> GetAllAsync(string roleName)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(SitePermissions.RoleName), roleName)
            );
        }

        public async Task<SitePermissions> GetAsync(string roleName, int siteId)
        {
            return await _repository.GetAsync(Q
                .Where(nameof(SitePermissions.RoleName), roleName)
                .Where(nameof(SitePermissions.SiteId), siteId)
            );
        }

        public async Task<bool> Exists(string roleName, int siteId)
        {
            return await _repository.ExistsAsync(Q
                .Where(nameof(SitePermissions.RoleName), roleName)
                .Where(nameof(SitePermissions.SiteId), siteId)
            );
        }

        public async Task<Dictionary<int, List<string>>> GetSitePermissionDictionaryAsync(IEnumerable<string> roles)
        {
            var sortedList = new Dictionary<int, List<string>>();
            if (roles == null) return sortedList;

            foreach (var roleName in roles)
            {
                var systemPermissionsList = await GetAllAsync(roleName);
                foreach (var systemPermissions in systemPermissionsList)
                {
                    if (systemPermissions.Permissions == null) continue;

                    var list = new List<string>();
                    foreach (var websitePermission in systemPermissions.Permissions)
                    {
                        if (!list.Contains(websitePermission)) list.Add(websitePermission);
                    }

                    if (sortedList.ContainsKey(systemPermissions.SiteId))
                    {
                        var permissions = sortedList[systemPermissions.SiteId];
                        foreach (var item in list)
                        {
                            if (!permissions.Contains(item)) permissions.Add(item);
                        }
                        sortedList[systemPermissions.SiteId] = permissions;
                    }
                    else
                    {
                        sortedList[systemPermissions.SiteId] = list;
                    }
                }
            }

            return sortedList;
        }

        public async Task<Dictionary<string, List<string>>> GetChannelPermissionDictionaryAsync(IList<string> roles)
        {
            var dict = new Dictionary<string, List<string>>();
            if (roles == null) return dict;

            foreach (var roleName in roles)
            {
                var systemPermissionsList = await GetAllAsync(roleName);
                foreach (var systemPermissions in systemPermissionsList)
                {
                    if (systemPermissions.ChannelIds == null) continue;

                    foreach (var channelId in systemPermissions.ChannelIds)
                    {
                        var key = AuthManager.GetPermissionDictKey(systemPermissions.SiteId, channelId);

                        if (!dict.TryGetValue(key, out var list))
                        {
                            list = new List<string>();
                            dict[key] = list;
                        }

                        if (systemPermissions.ChannelPermissions != null)
                        {
                            foreach (var channelPermission in systemPermissions.ChannelPermissions)
                            {
                                if (!list.Contains(channelPermission)) list.Add(channelPermission);
                            }
                        }
                    }
                }
            }

            return dict;
        }

        public async Task<Dictionary<string, List<string>>> GetContentPermissionDictionaryAsync(IList<string> roles)
        {
            var dict = new Dictionary<string, List<string>>();
            if (roles == null) return dict;

            foreach (var roleName in roles)
            {
                var systemPermissionsList = await GetAllAsync(roleName);
                foreach (var systemPermissions in systemPermissionsList)
                {
                    if (systemPermissions.ChannelIds == null) continue;

                    foreach (var channelId in systemPermissions.ChannelIds)
                    {
                        var key = AuthManager.GetPermissionDictKey(systemPermissions.SiteId, channelId);

                        if (!dict.TryGetValue(key, out var list))
                        {
                            list = new List<string>();
                            dict[key] = list;
                        }

                        if (systemPermissions.ContentPermissions != null)
                        {
                            foreach (var contentPermission in systemPermissions.ContentPermissions)
                            {
                                if (!list.Contains(contentPermission)) list.Add(contentPermission);
                            }
                        }
                    }
                }
            }

            return dict;
        }

        public async Task<List<string>> GetChannelPermissionListIgnoreChannelIdAsync(IList<string> roles)
        {
            var list = new List<string>();
            if (roles == null) return list;

            foreach (var roleName in roles)
            {
                var systemPermissionsList = await GetAllAsync(roleName);
                foreach (var systemPermissions in systemPermissionsList)
                {
                    if (systemPermissions.ChannelPermissions != null)
                    {
                        foreach (var channelPermission in systemPermissions.ChannelPermissions)
                        {
                            if (!list.Contains(channelPermission))
                            {
                                list.Add(channelPermission);
                            }
                        }
                    }
                }
            }

            return list;
        }
    }
}
