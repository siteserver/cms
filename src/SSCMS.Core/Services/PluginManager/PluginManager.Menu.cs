using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Datory.Utils;
using SSCMS;
using SSCMS.Utils;

namespace SSCMS.Core.Services.PluginManager
{
    public partial class PluginManager
    {
        public async Task<string> GetSystemDefaultPageUrlAsync(int siteId)
        {
            string pageUrl = null;

            foreach (var plugin in GetPlugins())
            {
                if (plugin.SystemDefaultPageUrl == null) continue;

                try
                {
                    pageUrl = GetMenuHref(plugin.PluginId, plugin.SystemDefaultPageUrl, siteId, 0, 0);
                }
                catch (Exception ex)
                {
                    await _databaseManager.ErrorLogRepository.AddErrorLogAsync(plugin.PluginId, ex);
                }
            }

            return pageUrl;
        }

        public async Task<string> GetHomeDefaultPageUrlAsync()
        {
            string pageUrl = null;

            foreach (var plugin in GetPlugins())
            {
                if (plugin.HomeDefaultPageUrl == null) continue;

                try
                {
                    pageUrl = GetMenuHref(plugin.PluginId, plugin.HomeDefaultPageUrl, 0, 0, 0);
                }
                catch (Exception ex)
                {
                    await _databaseManager.ErrorLogRepository.AddErrorLogAsync(plugin.PluginId, ex);
                }
            }

            return pageUrl;
        }

        public async Task<List<Menu>> GetTopMenusAsync()
        {
            var menus = new List<Menu>();

            var plugins = GetPlugins();

            foreach (var plugin in plugins)
            {
                try
                {
                    var systemMenus = plugin.GetSystemMenus() ?? await plugin.GetSystemMenusAsync();
                    if (systemMenus == null) continue;

                    var i = 0;
                    foreach (var systemMenu in systemMenus)
                    {
                        var pluginMenu = GetMenu(plugin.PluginId, 0, 0, 0, systemMenu, ++i);
                        menus.Add(pluginMenu);
                    }
                }
                catch (Exception ex)
                {
                    await _databaseManager.ErrorLogRepository.AddErrorLogAsync(plugin.PluginId, ex);
                }
            }

            return menus;
        }

        public async Task<List<Menu>> GetSiteMenusAsync(int siteId)
        {
            var menus = new List<Menu>();

            var plugins = GetPlugins();

            foreach (var plugin in plugins)
            {
                try
                {
                    var siteMenus = plugin.GetSiteMenus(siteId) ?? await plugin.GetSiteMenusAsync(siteId);
                    if (siteMenus == null) continue;

                    var i = 0;
                    foreach (var siteMenu in siteMenus)
                    {
                        var pluginMenu = GetMenu(plugin.PluginId, siteId, 0, 0, siteMenu, ++i);
                        menus.Add(pluginMenu);
                    }
                }
                catch (Exception ex)
                {
                    await _databaseManager.ErrorLogRepository.AddErrorLogAsync(plugin.PluginId, ex);
                }
            }

            return menus;
        }

        public async Task<List<Menu>> GetContentMenusAsync(List<string> pluginIds, Content content)
        {
            var menus = new List<Menu>();
            if (pluginIds == null || pluginIds.Count == 0) return menus;

            foreach (var plugin in GetPlugins())
            {
                if (!pluginIds.Contains(plugin.PluginId)) continue;

                try
                {
                    var contentMenus = plugin.GetContentMenus(content) ?? await plugin.GetContentMenusAsync(content);
                    if (contentMenus == null) continue;

                    var i = 0;
                    foreach (var contentMenu in contentMenus)
                    {
                        var pluginMenu = GetMenu(plugin.PluginId, content.SiteId, content.ChannelId, content.Id, contentMenu, ++i);
                        menus.Add(pluginMenu);
                    }
                }
                catch (Exception ex)
                {
                    await _databaseManager.ErrorLogRepository.AddErrorLogAsync(plugin.PluginId, ex);
                }
            }

            return menus;
        }

        private string GetMenuHref(string pluginId, string href, int siteId, int channelId, int contentId)
        {
            if (PageUtils.IsAbsoluteUrl(href))
            {
                return href;
            }

            var url = PageUtils.AddQueryStringIfNotExists(_pathManager.ParsePluginUrl(pluginId, href), new NameValueCollection
            {
                {"v", StringUtils.GetRandomInt(1, 1000).ToString()},
                {"pluginId", pluginId},
                {"apiUrl", _pathManager.InnerApiUrl}
            });
            if (siteId > 0)
            {
                url = PageUtils.AddQueryStringIfNotExists(url, new NameValueCollection
                {
                    {"siteId", siteId.ToString()}
                });
            }
            if (channelId > 0)
            {
                url = PageUtils.AddQueryStringIfNotExists(url, new NameValueCollection
                {
                    {"channelId", channelId.ToString()}
                });
            }
            if (contentId > 0)
            {
                url = PageUtils.AddQueryStringIfNotExists(url, new NameValueCollection
                {
                    {"contentId", contentId.ToString()}
                });
            }
            return url;
        }

        private Menu GetMenu(string pluginId, int siteId, int channelId, int contentId, Menu metadataMenu, int i)
        {
            var menu = new Menu
            {
                Id = metadataMenu.Id,
                Text = metadataMenu.Text,
                Link = metadataMenu.Link,
                Target = metadataMenu.Target,
                IconClass = metadataMenu.IconClass,
                PluginId = pluginId
            };

            if (string.IsNullOrEmpty(menu.Id))
            {
                menu.Id = pluginId + i;
            }
            if (!string.IsNullOrEmpty(menu.Link))
            {
                menu.Link = GetMenuHref(pluginId, menu.Link, siteId, channelId, contentId);
            }
            if (channelId == 0 && contentId == 0 && string.IsNullOrEmpty(menu.Target))
            {
                menu.Target = "right";
            }

            if (metadataMenu.Menus != null && metadataMenu.Menus.Count > 0)
            {
                var children = new List<Menu>();
                var x = 1;
                foreach (var childMetadataMenu in metadataMenu.Menus)
                {
                    var child = GetMenu(pluginId, siteId, channelId, contentId, childMetadataMenu, x++);

                    children.Add(child);
                }
                menu.Menus = children;
            }

            return menu;
        }

        public Tab GetPluginTab(string pluginId, string prefix, Menu menu)
        {
            var tab = new Tab
            {
                Id = menu.Id,
                Text = menu.Text,
                IconClass = menu.IconClass,
                Selected = false,
                Href = menu.Link,
                Target = menu.Target,
                Permissions = string.Empty
            };

            var permissions = new List<string>();
            if (menu.Menus != null && menu.Menus.Count > 0)
            {
                tab.Children = new Tab[menu.Menus.Count];
                for (var i = 0; i < menu.Menus.Count; i++)
                {
                    var child = menu.Menus[i];
                    var childPermission = GetMenuPermission(pluginId, menu.Text, child);
                    permissions.Add(childPermission);

                    tab.Children[i] = GetPluginTab(pluginId, menu.Text, child);
                }
            }
            else
            {
                var permission = GetMenuPermission(pluginId, prefix, menu);
                permissions.Add(permission);
            }
            tab.Permissions = Utilities.ToString(permissions);

            return tab;
        }

        public async Task<List<PermissionConfig>> GetTopPermissionsAsync()
        {
            var permissions = new List<PermissionConfig>();

            foreach (var plugin in GetPlugins())
            {
                try
                {
                    var systemMenus = plugin.GetSystemMenus() ?? await plugin.GetSystemMenusAsync();
                    if (systemMenus == null) continue;

                    permissions.Add(new PermissionConfig(plugin.PluginId, $"系统管理 -> {plugin.Name}（插件）"));
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(plugin.PluginId, ex);
                }
            }

            return permissions;
        }

        private string GetMenuPermission(string pluginId, string prefix, Menu menu)
        {
            return string.IsNullOrEmpty(prefix) ? $"{pluginId}:{menu.Text}" : $"{pluginId}:{prefix}:{menu.Text}";
        }

        public async Task<List<PermissionConfig>> GetSitePermissionsAsync(int siteId)
        {
            var permissions = new List<PermissionConfig>();

            foreach (var plugin in GetPlugins())
            {
                try
                {
                    var siteMenus = plugin.GetSiteMenus(siteId) ?? await plugin.GetSiteMenusAsync(siteId);
                    if (siteMenus == null) continue;

                    foreach (var siteMenu in siteMenus)
                    {
                        if (siteMenu == null) continue;

                        if (siteMenu.Menus != null && siteMenu.Menus.Count > 0)
                        {
                            foreach (var menu in siteMenu.Menus)
                            {
                                var permission = GetMenuPermission(plugin.PluginId, siteMenu.Text, menu);
                                permissions.Add(new PermissionConfig(permission, $"{plugin.Name} -> {menu.Text}"));
                            }
                        }
                        else
                        {
                            var permission = GetMenuPermission(plugin.PluginId, string.Empty, siteMenu);
                            permissions.Add(new PermissionConfig(permission, $"{plugin.Name} -> {siteMenu.Text}"));
                        }
                    }
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(plugin.PluginId, ex);
                }
            }

            return permissions;
        }
    }
}
