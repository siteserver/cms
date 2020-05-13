using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SiteServer.CMS.Api;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Plugin
{
    public static class PluginMenuManager
    {
        public static string GetSystemDefaultPageUrl(int siteId)
        {
            string pageUrl = null;

            foreach (var service in PluginManager.Services)
            {
                if (service.SystemDefaultPageUrl == null) continue;

                try
                {
                    pageUrl = GetMenuHref(service.PluginId, service.SystemDefaultPageUrl, siteId, 0, 0);
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(service.PluginId, ex);
                }
            }

            return pageUrl;
        }

        public static string GetHomeDefaultPageUrl()
        {
            string pageUrl = null;

            foreach (var service in PluginManager.Services)
            {
                if (service.HomeDefaultPageUrl == null) continue;

                try
                {
                    pageUrl = GetMenuHref(service.PluginId, service.HomeDefaultPageUrl, 0, 0, 0);
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(service.PluginId, ex);
                }
            }

            return pageUrl;
        }

        public static List<PluginMenu> GetTopMenus()
        {
            var menus = new List<PluginMenu>();

            foreach (var service in PluginManager.Services)
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

                    foreach (var metadataMenu in metadataMenus)
                    {
                        var pluginMenu = GetMenu(service.PluginId, 0, 0, 0, metadataMenu);
                        menus.Add(pluginMenu);
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(service.PluginId, ex);
                }
            }

            return menus;
        }

        public static List<PluginMenu> GetSiteMenus(int siteId)
        {
            var menus = new List<PluginMenu>();

            foreach (var service in PluginManager.Services)
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

                    foreach (var metadataMenu in metadataMenus)
                    {
                        var pluginMenu = GetMenu(service.PluginId, siteId, 0, 0, metadataMenu);
                        menus.Add(pluginMenu);
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(service.PluginId, ex);
                }
            }

            return menus;
        }

        public static List<PluginMenu> GetHomeMenus()
        {
            var menus = new List<PluginMenu>();

            foreach (var service in PluginManager.Services)
            {
                if (service.HomeMenuFuncs == null) continue;

                try
                {
                    var metadataMenus = new List<Menu>();

                    foreach (var menuFunc in service.HomeMenuFuncs)
                    {
                        var metadataMenu = menuFunc.Invoke();
                        if (metadataMenu != null)
                        {
                            metadataMenus.Add(metadataMenu);
                        }
                    }

                    if (metadataMenus.Count == 0) continue;

                    foreach (var metadataMenu in metadataMenus)
                    {
                        var pluginMenu = GetMenu(service.PluginId, 0, 0, 0, metadataMenu);
                        menus.Add(pluginMenu);
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(service.PluginId, ex);
                }
            }

            return menus;
        }

        public static List<PluginMenu> GetContentMenus(List<string> pluginIds, ContentInfo contentInfo)
        {
            var menus = new List<PluginMenu>();
            if (pluginIds == null || pluginIds.Count == 0) return menus;

            foreach (var service in PluginManager.Services)
            {
                if (!pluginIds.Contains(service.PluginId)) continue;

                if (service.ContentMenuFuncs == null) continue;

                try
                {
                    var metadataMenus = new List<Menu>();

                    foreach (var menuFunc in service.ContentMenuFuncs)
                    {
                        var metadataMenu = menuFunc.Invoke(contentInfo);
                        if (metadataMenu != null)
                        {
                            metadataMenus.Add(metadataMenu);
                        }
                    }

                    if (metadataMenus.Count == 0) continue;

                    foreach (var metadataMenu in metadataMenus)
                    {
                        var pluginMenu = GetMenu(service.PluginId, contentInfo.SiteId, contentInfo.ChannelId, contentInfo.Id, metadataMenu);
                        menus.Add(pluginMenu);
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(service.PluginId, ex);
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

        //public static string GetMenuContentHref(string pluginId, string href, int siteId, int channelId, int contentId, string returnUrl)
        //{
        //    if (PageUtils.IsAbsoluteUrl(href))
        //    {
        //        return href;
        //    }
        //    return PageUtils.AddQueryStringIfNotExists(PageUtils.ParsePluginUrl(pluginId, href), new NameValueCollection
        //    {
        //        {"siteId", siteId.ToString()},
        //        {"channelId", channelId.ToString()},
        //        {"contentId", contentId.ToString()},
        //        {"apiUrl", ApiManager.ApiUrl},
        //        {"returnUrl", returnUrl},
        //        {"v", StringUtils.GetRandomInt(1, 1000).ToString()}
        //    });
        //}

        //public static string GetMenuContentHrefPrefix(string pluginId, string href)
        //{
        //    if (PageUtils.IsAbsoluteUrl(href))
        //    {
        //        return href;
        //    }
        //    return PageUtils.AddQueryStringIfNotExists(PageUtils.ParsePluginUrl(pluginId, href), new NameValueCollection
        //    {
        //        {"apiUrl", ApiManager.ApiUrl},
        //        {"v", StringUtils.GetRandomInt(1, 1000).ToString()}
        //    });
        //}

        private static PluginMenu GetMenu(string pluginId, int siteId, int channelId, int contentId, Menu metadataMenu)
        {
            var menu = new PluginMenu
            {
                Id = metadataMenu.Id,
                Text = metadataMenu.Text,
                Href = metadataMenu.Href,
                Target = metadataMenu.Target,
                IconClass = metadataMenu.IconClass,
                PluginId = pluginId
            };

            if (string.IsNullOrEmpty(menu.Id))
            {
                menu.Id = metadataMenu.Text;
            }
            if (!string.IsNullOrEmpty(menu.Href))
            {
                menu.Href = GetMenuHref(pluginId, menu.Href, siteId, channelId, contentId);
            }
            if (channelId == 0 && contentId == 0 && string.IsNullOrEmpty(menu.Target))
            {
                menu.Target = "right";
            }

            if (metadataMenu.Menus != null && metadataMenu.Menus.Count > 0)
            {
                var children = new List<Menu>();
                foreach (var childMetadataMenu in metadataMenu.Menus)
                {
                    var child = GetMenu(pluginId, siteId, channelId, contentId, childMetadataMenu);

                    children.Add(child);
                }
                menu.Menus = children;
            }

            return menu;
        }

        public static Tab GetPluginTab(string pluginId, Menu parent, Menu menu)
        {
            var tab = new Tab
            {
                Id = menu.Id,
                Text = menu.Text,
                IconClass = menu.IconClass,
                Selected = false,
                Href = menu.Href,
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
                    var childPermission = GetMenuPermission(pluginId, menu, child);
                    permissions.Add(childPermission);

                    tab.Children[i] = GetPluginTab(pluginId, menu, child);
                }
            }
            else
            {
                var permission = GetMenuPermission(pluginId, parent, menu);
                permissions.Add(permission);
            }
            tab.Permissions = TranslateUtils.ObjectCollectionToString(permissions);

            return tab;
        }

        public static List<PermissionConfigManager.PermissionConfig> GetTopPermissions()
        {
            var permissions = new List<PermissionConfigManager.PermissionConfig>();

            foreach (var service in PluginManager.Services)
            {
                if (service.SystemMenuFuncs != null)
                {
                    permissions.Add(new PermissionConfigManager.PermissionConfig(service.PluginId, $"系统管理 -> {service.Metadata.Title}（插件）"));
                }
            }

            return permissions;
        }

        private static string GetMenuPermission(string pluginId, Menu parent, Menu menu)
        {
            var prefix = string.Empty;
            if (parent != null)
            {
                prefix = string.IsNullOrEmpty(parent.Id) ? $":{parent.Text}" : $":{parent.Id}";
            }
            return string.IsNullOrEmpty(menu.Id) ? $"{pluginId}{prefix}:{menu.Text}" : $"{pluginId}{prefix}:{menu.Id}";
        }

        public static List<PermissionConfigManager.PermissionConfig> GetSitePermissions(int siteId)
        {
            var permissions = new List<PermissionConfigManager.PermissionConfig>();

            foreach (var service in PluginManager.Services)
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
                            var permission = GetMenuPermission(service.PluginId, metadataMenu, menu);
                            permissions.Add(new PermissionConfigManager.PermissionConfig(permission, $"{service.Metadata.Title} -> {menu.Text}"));
                        }
                    }
                    else
                    {
                        var permission = GetMenuPermission(service.PluginId, null, metadataMenu);
                        permissions.Add(new PermissionConfigManager.PermissionConfig(permission, $"{service.Metadata.Title} -> {metadataMenu.Text}"));
                    }
                }
            }

            return permissions;
        }
    }
}
