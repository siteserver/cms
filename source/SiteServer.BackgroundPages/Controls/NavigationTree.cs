using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.Tabs;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Controls
{
    public class NavigationTree : Control
    {
        protected override void Render(HtmlTextWriter writer)
        {
            if (string.IsNullOrEmpty(TopId)) return;

            var builder = new StringBuilder();
            BuildNavigationTree(builder, GetTabList(), 0, true);
            writer.Write(builder);
        }

        private static Tab GetPluginTab(PluginMenu menu)
        {
            var tab = new Tab
            {
                Id = menu.Id,
                Text = menu.Text,
                IconUrl = menu.IconUrl,
                Selected = menu.Selected,
                Href = menu.Href,
                Target = menu.Target,
                //Permissions = TranslateUtils.ObjectCollectionToString(menu.Permissions)
            };
            if (menu.Menus != null && menu.Menus.Count > 0)
            {
                tab.Children = new Tab[menu.Menus.Count];
                for (var i = 0; i < menu.Menus.Count; i++)
                {
                    tab.Children[i] = GetPluginTab(menu.Menus[i]);
                }
            }
            return tab;
        }

        /// <summary>
        /// Returns the current instance of the TabCollection
        /// </summary>
        /// <returns></returns>
        protected List<Tab> GetTabList()
        {
            var directoryPath = PathUtils.GetSiteFilesPath(DirectoryUtils.SiteFiles.Configuration, "Menus");
            var filePath = PathUtils.Combine(directoryPath, $"{TopId}.config");

            var tabs = CacheUtils.Get(filePath) as List<Tab>;
            if (tabs != null) return tabs;

            tabs = new List<Tab>();

            var tabCollection = (TabCollection)Serializer.ConvertFileToObject(filePath, typeof(TabCollection));
            
            if (tabCollection?.Tabs != null)
            {
                tabs = tabCollection.Tabs.ToList();
            }

            var menus = PluginManager.GetAllMenus(TopId, PublishmentSystemId);
            foreach (var menu in menus)
            {
                if (string.IsNullOrEmpty(menu.ParentId))
                {
                    var isExists = false;
                    foreach (var childTab in tabs)
                    {
                        if (childTab.Id == menu.Id)
                        {
                            isExists = true;
                        }
                    }

                    if (isExists) continue;

                    tabs.Add(GetPluginTab(menu));
                }
                else
                {
                    foreach (var tab in tabs)
                    {
                        if (!StringUtils.EqualsIgnoreCase(menu.ParentId, tab.Id)) continue;

                        var isExists = false;
                        foreach (var childTab in tab.Children)
                        {
                            if (childTab.Id == menu.Id)
                            {
                                isExists = true;
                            }
                        }

                        if (isExists) continue;

                        var list = new List<Tab>();
                        if (tab.Children != null)
                        {
                            list = tab.Children.ToList();
                        }
                        list.Add(GetPluginTab(menu));
                        tab.Children = list.ToArray();
                    }
                }
            }

            CacheUtils.Max(filePath, tabs, new CacheDependency(filePath));

            return tabs;
        }

        /// <summary>
        /// Creates the markup for the current TabCollection
        /// </summary>
        /// <returns></returns>
        protected void BuildNavigationTree(StringBuilder builder, List<Tab> tabs, int parentsCount, bool isDisplay)
        {
            if (tabs == null) return;

            foreach (var parent in tabs)
            {
                if (!TabManager.IsValid(parent, PermissionList)) continue;

                var linkUrl = FormatLink(parent);
                //if (!string.IsNullOrEmpty(linkUrl) && !StringUtils.EqualsIgnoreCase(linkUrl, PageUtils.UnclickedUrl))
                //{
                //    linkUrl = PageUtils.GetLoadingUrl(linkUrl);
                //}
                var hasChildren = parent.Children != null && parent.Children.Length > 0;
                var openWindow = !hasChildren && StringUtils.EndsWithIgnoreCase(parent.Href, "main.aspx");

                var item = NavigationTreeItem.CreateNavigationBarItem(isDisplay, parent.Selected, parentsCount, hasChildren, openWindow, parent.Text, linkUrl, parent.Target, parent.Enabled, parent.IconUrl);

                builder.Append(item.GetTrHtml());

                if (PublishmentSystemId > 0)
                {
                    var tabCollection = NodeNaviTabManager.GetTabCollection(parent, PublishmentSystemId);
                    if (tabCollection?.Tabs != null && tabCollection.Tabs.Length > 0)
                    {
                        BuildNavigationTree(builder, tabCollection.Tabs.ToList(), parentsCount + 1, parent.Selected);
                    }
                }
                else
                {
                    if (parent.Children != null && parent.Children.Length > 0)
                    {
                        BuildNavigationTree(builder, parent.Children.ToList(), parentsCount + 1, parent.Selected);
                    }
                }
            }
        }

        public int PublishmentSystemId
        {
            get
            {
                var state = ViewState["PublishmentSystemId"];
                if (state == null)
                {
                    return 0;
                }
                return (int)state;
            }
            set
            {
                ViewState["PublishmentSystemId"] = value;
            }
        }

        /// <summary>
        /// returns the file name containing the tab configuration
        /// </summary>
        public string TopId
        {
            get
            {
                var state = ViewState["TopId"];
                if (state == null)
                {
                    return string.Empty;
                }
                return (string)state;
            }
            set
            {
                ViewState["TopId"] = value;
            }
        }

        public List<string> PermissionList
        {
            get
            {
                var state = ViewState["PermissionList"];
                if (state == null)
                {
                    return new List<string>();
                }
                return (List<string>)state;
            }
            set
            {
                ViewState["PermissionList"] = value;
            }
        }

        /// <summary>
        /// Resolves the current url and attempts to append the specified querystring
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected string FormatLink(Tab t)
        {
            if (!t.HasHref)
                return null;

            var url = t.KeepQueryString ? PageUtils.AddQueryString(t.Href, Context.Request.QueryString) : t.Href;

            return url;

            //return ResolveUrl(url);
        }

        protected virtual string GetText(Tab t)
        {
            return t.Text;
        }
    }
}
