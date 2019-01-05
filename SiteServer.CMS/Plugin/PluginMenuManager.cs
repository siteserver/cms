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

        private static string GetMenuId(string serviceId, int i)
        {
            return $"{serviceId}_{i}";
        }

        public static Dictionary<string, Menu> GetTopMenus()
        {
            var menus = new Dictionary<string, Menu>();

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

                    var i = 0;
                    foreach (var metadataMenu in metadataMenus)
                    {
                        var pluginMenu = GetMenu(service.PluginId, 0, 0, 0, metadataMenu, 0);
                        menus[GetMenuId(service.PluginId, ++i)] = pluginMenu;
                    }
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
                        var pluginMenu = GetMenu(service.PluginId, siteId, 0, 0, metadataMenu, 0);
                        menus[GetMenuId(service.PluginId, ++i)] = pluginMenu;
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(service.PluginId, ex);
                }
            }

            return menus;
        }

        public static List<Menu> GetContentMenus(List<string> pluginIds, ContentInfo contentInfo)
        {
            var menus = new List<Menu>();
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
                        var pluginMenu = GetMenu(service.PluginId, contentInfo.SiteId, contentInfo.ChannelId, contentInfo.Id, metadataMenu, 0);
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

        private static Menu GetMenu(string pluginId, int siteId, int channelId, int contentId, Menu metadataMenu, int i)
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
                menu.Href = GetMenuHref(pluginId, menu.Href, siteId, channelId, contentId);
            }
            if (channelId == 0 && contentId == 0 && string.IsNullOrEmpty(menu.Target))
            {
                menu.Target = "right";
            }

            if (metadataMenu.Menus != null && metadataMenu.Menus.Count > 0)
            {
                var chlildren = new List<Menu>();
                var x = 1;
                foreach (var childMetadataMenu in metadataMenu.Menus)
                {
                    var child = GetMenu(pluginId, siteId, channelId, contentId, childMetadataMenu, x++);

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
                if (service.SystemMenuFuncs != null)
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
                if (service.SiteMenuFuncs != null)
                {
                    permissions.Add(new PermissionConfigManager.PermissionConfig(service.PluginId, $"{service.Metadata.Title}"));
                }
            }

            return permissions;
        }
    }
}
