using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class OldPluginManager
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

            //var url = PageUtils.AddQueryStringIfNotExists(_pathManager.ParsePluginUrl(pluginId, href), new NameValueCollection
            //{
            //    {"v", StringUtils.GetRandomInt(1, 1000).ToString()},
            //    {"pluginId", pluginId},
            //    {"apiUrl", _pathManager.InnerApiUrl}
            //});
            var url = string.Empty;
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
                IconClass = metadataMenu.IconClass
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

            if (metadataMenu.Children != null && metadataMenu.Children.Count > 0)
            {
                var children = new List<Menu>();
                var x = 1;
                foreach (var childMetadataMenu in metadataMenu.Children)
                {
                    var child = GetMenu(pluginId, siteId, channelId, contentId, childMetadataMenu, x++);

                    children.Add(child);
                }
                menu.Children = children;
            }

            return menu;
        }

        public Menu GetPluginTab(string pluginId, string prefix, Menu menu)
        {
            var tab = new Menu
            {
                Id = menu.Id,
                Text = menu.Text,
                IconClass = menu.IconClass,
                Link = menu.Link,
                Target = menu.Target
            };

            var permissions = new List<string>();
            if (menu.Children != null && menu.Children.Count > 0)
            {
                tab.Children = new List<Menu>();
                for (var i = 0; i < menu.Children.Count; i++)
                {
                    var child = menu.Children[i];
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
            tab.Permissions = permissions;

            return tab;
        }

        public async Task<List<Permission>> GetTopPermissionsAsync()
        {
            var permissions = new List<Permission>();

            foreach (var plugin in GetPlugins())
            {
                try
                {
                    var systemMenus = plugin.GetSystemMenus() ?? await plugin.GetSystemMenusAsync();
                    if (systemMenus == null) continue;

                    permissions.Add(new Permission
                    {
                        Id = plugin.PluginId,
                        Text = $"系统管理 -> {plugin.Name}（插件）"
                    });
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

        public async Task<List<Permission>> GetSitePermissionsAsync(int siteId)
        {
            var permissions = new List<Permission>();

            foreach (var plugin in GetPlugins())
            {
                try
                {
                    var siteMenus = plugin.GetSiteMenus(siteId) ?? await plugin.GetSiteMenusAsync(siteId);
                    if (siteMenus == null) continue;

                    foreach (var siteMenu in siteMenus)
                    {
                        if (siteMenu == null) continue;

                        if (siteMenu.Children != null && siteMenu.Children.Count > 0)
                        {
                            foreach (var menu in siteMenu.Children)
                            {
                                var permission = GetMenuPermission(plugin.PluginId, siteMenu.Text, menu);
                                permissions.Add(new Permission
                                {
                                    Id = permission,
                                    Text = $"{plugin.Name} -> {menu.Text}"
                                });
                            }
                        }
                        else
                        {
                            var permission = GetMenuPermission(plugin.PluginId, string.Empty, siteMenu);
                            permissions.Add(new Permission
                            {
                                Id = permission,
                                Text = $"{plugin.Name} -> {siteMenu.Text}"
                            });
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
