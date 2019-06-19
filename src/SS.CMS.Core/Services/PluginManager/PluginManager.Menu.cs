using System;
using System.Collections.Generic;
using SS.CMS.Core.Common;
using SS.CMS.Models;
using SS.CMS.Services.IUrlManager;

namespace SS.CMS.Core.Services
{
    public partial class PluginManager
    {
        public List<Menu> GetTopMenus(IUrlManager urlManager)
        {
            var menus = new List<Menu>();

            foreach (var service in Services)
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
                        var pluginMenu = GetMenu(urlManager, service.PluginId, 0, 0, 0, metadataMenu, ++i);
                        menus.Add(pluginMenu);
                    }
                }
                catch (Exception ex)
                {
                    _errorLogRepository.AddErrorLog(service.PluginId, ex);
                }
            }

            return menus;
        }

        public List<Menu> GetSiteMenus(IUrlManager urlManager, int siteId)
        {
            var menus = new List<Menu>();

            foreach (var service in Services)
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
                        var pluginMenu = GetMenu(urlManager, service.PluginId, siteId, 0, 0, metadataMenu, ++i);
                        menus.Add(pluginMenu);
                    }
                }
                catch (Exception ex)
                {
                    _errorLogRepository.AddErrorLog(service.PluginId, ex);
                }
            }

            return menus;
        }

        public List<Menu> GetContentMenus(IUrlManager urlManager, List<string> pluginIds, ContentInfo contentInfo)
        {
            var menus = new List<Menu>();
            if (pluginIds == null || pluginIds.Count == 0) return menus;

            foreach (var service in Services)
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

                    var i = 0;
                    foreach (var metadataMenu in metadataMenus)
                    {
                        var pluginMenu = GetMenu(urlManager, service.PluginId, contentInfo.SiteId, contentInfo.ChannelId, contentInfo.Id, metadataMenu, ++i);
                        menus.Add(pluginMenu);
                    }
                }
                catch (Exception ex)
                {
                    _errorLogRepository.AddErrorLog(service.PluginId, ex);
                }
            }

            return menus;
        }

        private Menu GetMenu(IUrlManager urlManager, string pluginId, int siteId, int channelId, int contentId, Menu metadataMenu, int i)
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
                menu.Link = urlManager.GetMenuUrl(pluginId, menu.Link, siteId, channelId, contentId);
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
                    var child = GetMenu(urlManager, pluginId, siteId, channelId, contentId, childMetadataMenu, x++);

                    chlildren.Add(child);
                }
                menu.Menus = chlildren;
            }

            return menu;
        }

        public List<Permission> GetTopPermissions()
        {
            var permissions = new List<Permission>();

            foreach (var service in Services)
            {
                if (service.SystemMenuFuncs != null)
                {
                    permissions.Add(new Permission
                    {
                        Id = service.PluginId,
                        Text = $"系统管理 -> {service.Metadata.Title}（插件）"
                    });
                }
            }

            return permissions;
        }

        public List<Permission> GetSitePermissions(int siteId)
        {
            var permissions = new List<Permission>();

            foreach (var service in Services)
            {
                if (service.SiteMenuFuncs != null)
                {
                    permissions.Add(new Permission
                    {
                        Id = service.PluginId,
                        Text = service.Metadata.Title
                    });
                }
            }

            return permissions;
        }
    }
}
