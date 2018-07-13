using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SiteServer.CMS.Api;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Plugin
{
    public static class PluginMenuManager
    {
        public static Dictionary<string, Menu> GetTopMenus()
        {
            var menus = new Dictionary<string, Menu>();

            foreach (var service in PluginManager.Services)
            {
                if (service.PluginMenu == null) continue;

                try
                {
                    var pluginMenu = GetMenu(service.PluginId, 0, service.PluginMenu, 0);

                    menus.Add(service.PluginId, pluginMenu);
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(service.PluginId, ex);
                }
            }

            return menus;
        }

        public static Dictionary<string, Menu> GetSiteMenus(int siteId)
        {
            var menus = new Dictionary<string, Menu>();

            foreach (var service in PluginManager.Services)
            {
                if (service.SiteMenuFunc == null) continue;

                Menu metadataMenu = null;
                try
                {
                    metadataMenu = service.SiteMenuFunc.Invoke(siteId);
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(service.PluginId, ex);
                }

                if (metadataMenu == null) continue;

                var pluginMenu = GetMenu(service.PluginId, siteId, metadataMenu, 0);

                menus.Add(service.PluginId, pluginMenu);
            }

            return menus;
        }

        public static string GetMenuHref(string pluginId, string href, int siteId)
        {
            if (PageUtils.IsAbsoluteUrl(href))
            {
                return href;
            }

            var url = PageUtils.AddQueryStringIfNotExists(PageUtils.ParsePluginUrl(pluginId, href), new NameValueCollection
            {
                {"v", StringUtils.GetRandomInt(1, 1000).ToString()},
                {"apiUrl", ApiManager.ApiUrl}
            });
            if (siteId > 0)
            {
                url = PageUtils.AddQueryStringIfNotExists(url, new NameValueCollection
                {
                    {"siteId", siteId.ToString()}
                });
            }
            return url;
        }

        public static string GetMenuContentHref(string pluginId, string href, int siteId, int channelId, int contentId, string returnUrl)
        {
            if (PageUtils.IsAbsoluteUrl(href))
            {
                return href;
            }
            return PageUtils.AddQueryStringIfNotExists(PageUtils.ParsePluginUrl(pluginId, href), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"apiUrl", ApiManager.ApiUrl},
                {"returnUrl", returnUrl},
                {"v", StringUtils.GetRandomInt(1, 1000).ToString()}
            });
        }

        private static Menu GetMenu(string pluginId, int siteId, Menu metadataMenu, int i)
        {
            var menu = new Menu
            {
                Id = metadataMenu.Id,
                Text = metadataMenu.Text,
                Href = metadataMenu.Href,
                Target = metadataMenu.Target,
                IconClass = metadataMenu.IconClass
            };

            if (string.IsNullOrEmpty(menu.Id))
            {
                menu.Id = pluginId + i;
            }
            if (!string.IsNullOrEmpty(menu.Href))
            {
                menu.Href = GetMenuHref(pluginId, menu.Href, siteId);
            }
            if (string.IsNullOrEmpty(menu.Target))
            {
                menu.Target = "right";
            }

            if (metadataMenu.Menus != null && metadataMenu.Menus.Count > 0)
            {
                var chlildren = new List<Menu>();
                var x = 1;
                foreach (var childMetadataMenu in metadataMenu.Menus)
                {
                    var child = GetMenu(pluginId, siteId, childMetadataMenu, x++);

                    chlildren.Add(child);
                }
                menu.Menus = chlildren;
            }

            return menu;
        }

        public static List<PermissionConfigManager.PermissionConfig> GetTopPermissions()
        {
            var permissions = new List<PermissionConfigManager.PermissionConfig>();

            foreach (var service in PluginManager.Services)
            {
                if (service.PluginMenu != null)
                {
                    permissions.Add(new PermissionConfigManager.PermissionConfig(service.PluginId, $"系统管理 -> {service.Metadata.Title}（插件）"));
                }
            }

            return permissions;
        }

        public static List<PermissionConfigManager.PermissionConfig> GetSitePermissions(int siteId)
        {
            var permissions = new List<PermissionConfigManager.PermissionConfig>();

            foreach (var service in PluginManager.Services)
            {
                if (service.SiteMenuFunc != null)
                {
                    permissions.Add(new PermissionConfigManager.PermissionConfig(service.PluginId, $"{service.Metadata.Title}"));
                }
            }

            return permissions;
        }
    }
}
