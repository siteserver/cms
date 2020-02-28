using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Datory.Utils;
using SS.CMS.Abstractions;
using SS.CMS.Api;
using SS.CMS.Core;

namespace SS.CMS.Plugins
{
    public static class PluginMenuManager
    {

        public static async Task<string> GetSystemDefaultPageUrlAsync(int siteId)
        {
            string pageUrl = null;

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                if (service.SystemDefaultPageUrl == null) continue;

                try
                {
                    pageUrl = GetMenuHref(service.PluginId, service.SystemDefaultPageUrl, siteId, 0, 0);
                }
                catch (Exception ex)
                {
                    await GlobalSettings.ErrorLogRepository.AddErrorLogAsync(service.PluginId, ex);
                }
            }

            return pageUrl;
        }

        public static async Task<string> GetHomeDefaultPageUrlAsync()
        {
            string pageUrl = null;

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                if (service.HomeDefaultPageUrl == null) continue;

                try
                {
                    pageUrl = GetMenuHref(service.PluginId, service.HomeDefaultPageUrl, 0, 0, 0);
                }
                catch (Exception ex)
                {
                    await GlobalSettings.ErrorLogRepository.AddErrorLogAsync(service.PluginId, ex);
                }
            }

            return pageUrl;
        }

        public static async Task<List<Menu>> GetTopMenusAsync()
        {
            var menus = new List<Menu>();

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                if (service.SystemMenuFuncs == null) continue;

                try
                {
                    var metadataMenus = new List<Menu>();

                    foreach (var menuFunc in service.SystemMenuFuncs)
                    {
                        var metadataMenu = menuFunc.Invoke();
                        if (metadataMenu != null)
                        {
                            metadataMenus.Add(metadataMenu);
                        }
                    }

                    if (metadataMenus.Count == 0) continue;

                    var i = 0;
                    foreach (var metadataMenu in metadataMenus)
                    {
                        var pluginMenu = GetMenu(service.PluginId, 0, 0, 0, metadataMenu, ++i);
                        menus.Add(pluginMenu);
                    }
                }
                catch (Exception ex)
                {
                    await GlobalSettings.ErrorLogRepository.AddErrorLogAsync(service.PluginId, ex);
                }
            }

            return menus;
        }

        public static async Task<List<Menu>> GetSiteMenusAsync(int siteId)
        {
            var menus = new List<Menu>();

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                if (service.SiteMenuFuncs == null) continue;

                try
                {
                    var metadataMenus = new List<Menu>();

                    foreach (var menuFunc in service.SiteMenuFuncs)
                    {
                        var metadataMenu = menuFunc.Invoke(siteId);
                        if (metadataMenu != null)
                        {
                            metadataMenus.Add(metadataMenu);
                        }
                    }

                    if (metadataMenus.Count == 0) continue;

                    var i = 0;
                    foreach (var metadataMenu in metadataMenus)
                    {
                        var pluginMenu = GetMenu(service.PluginId, siteId, 0, 0, metadataMenu, ++i);
                        menus.Add(pluginMenu);
                    }
                }
                catch (Exception ex)
                {
                    await GlobalSettings.ErrorLogRepository.AddErrorLogAsync(service.PluginId, ex);
                }
            }

            return menus;
        }

        public static async Task<List<Menu>> GetContentMenusAsync(List<string> pluginIds, Content content)
        {
            var menus = new List<Menu>();
            if (pluginIds == null || pluginIds.Count == 0) return menus;

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                if (!pluginIds.Contains(service.PluginId)) continue;

                if (service.ContentMenuFuncs == null) continue;

                try
                {
                    var metadataMenus = new List<Menu>();

                    foreach (var menuFunc in service.ContentMenuFuncs)
                    {
                        var metadataMenu = menuFunc.Invoke(content);
                        if (metadataMenu != null)
                        {
                            metadataMenus.Add(metadataMenu);
                        }
                    }

                    if (metadataMenus.Count == 0) continue;

                    var i = 0;
                    foreach (var metadataMenu in metadataMenus)
                    {
                        var pluginMenu = GetMenu(service.PluginId, content.SiteId, content.ChannelId, content.Id, metadataMenu, ++i);
                        menus.Add(pluginMenu);
                    }
                }
                catch (Exception ex)
                {
                    await GlobalSettings.ErrorLogRepository.AddErrorLogAsync(service.PluginId, ex);
                }
            }

            return menus;
        }

        private static string GetMenuHref(string pluginId, string href, int siteId, int channelId, int contentId)
        {
            if (PageUtils.IsAbsoluteUrl(href))
            {
                return href;
            }

            var url = PageUtils.AddQueryStringIfNotExists(PageUtils.ParsePluginUrl(pluginId, href), new NameValueCollection
            {
                {"v", StringUtils.GetRandomInt(1, 1000).ToString()},
                {"pluginId", pluginId},
                {"apiUrl", ApiManager.InnerApiUrl}
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

        private static Menu GetMenu(string pluginId, int siteId, int channelId, int contentId, Menu metadataMenu, int i)
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

        public static Tab GetPluginTab(string pluginId, string prefix, Menu menu)
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

        public static async Task<List<PermissionConfigManager.PermissionConfig>> GetTopPermissionsAsync()
        {
            var permissions = new List<PermissionConfigManager.PermissionConfig>();

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                if (service.SystemMenuFuncs != null)
                {
                    permissions.Add(new PermissionConfigManager.PermissionConfig(service.PluginId, $"系统管理 -> {service.Metadata.Title}（插件）"));
                }
            }

            return permissions;
        }

        private static string GetMenuPermission(string pluginId, string prefix, Menu menu)
        {
            return string.IsNullOrEmpty(prefix) ? $"{pluginId}:{menu.Text}" : $"{pluginId}:{prefix}:{menu.Text}";
        }

        public static async Task<List<PermissionConfigManager.PermissionConfig>> GetSitePermissionsAsync(int siteId)
        {
            var permissions = new List<PermissionConfigManager.PermissionConfig>();

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                if (service.SiteMenuFuncs == null) continue;

                foreach (var menuFunc in service.SiteMenuFuncs)
                {
                    var metadataMenu = menuFunc.Invoke(siteId);
                    if (metadataMenu == null) continue;

                    if (metadataMenu.Menus != null && metadataMenu.Menus.Count > 0)
                    {
                        foreach (var menu in metadataMenu.Menus)
                        {
                            var permission = GetMenuPermission(service.PluginId, metadataMenu.Text, menu);
                            permissions.Add(new PermissionConfigManager.PermissionConfig(permission, $"{service.Metadata.Title} -> {menu.Text}"));
                        }
                    }
                    else
                    {
                        var permission = GetMenuPermission(service.PluginId, string.Empty, metadataMenu);
                        permissions.Add(new PermissionConfigManager.PermissionConfig(permission, $"{service.Metadata.Title} -> {metadataMenu.Text}"));
                    }
                }
            }

            return permissions;
        }
    }
}
