using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using SiteServer.Utils;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Controls
{
    public class NavigationTree : Control
    {
        protected override void Render(HtmlTextWriter writer)
        {
            var builder = new StringBuilder();
            var tabList = TabManager.GetTabList(TopId, SiteId);
            if (tabList != null && tabList.Count > 0)
            {
                builder.Append($@"<li class=""text-muted menu-title"">{Title}</li>");
                BuildNavigationTree(builder, tabList, 0, true);
            }
            writer.Write(builder);
        }

        protected void BuildNavigationTree(StringBuilder builder, List<Tab> tabs, int parentsCount, bool isDisplay)
        {
            if (tabs == null) return;

            foreach (var parent in tabs)
            {
                if (!TabManager.IsValid(parent, PermissionList)) continue;

                //var linkUrl = PageUtils.AddQueryString(parent.Href, new NameValueCollection { { "siteId", SiteId.ToString() } });
                //if (!string.IsNullOrEmpty(linkUrl) && !StringUtils.EqualsIgnoreCase(linkUrl, PageUtils.UnclickedUrl))
                //{
                //    linkUrl = PageUtils.GetLoadingUrl(linkUrl);
                //}
                var childBuilder = new StringBuilder();
                if (parent.Children != null && parent.Children.Length > 0)
                {
                    var tabCollection = new TabCollection(parent.Children);
                    if (tabCollection.Tabs != null && tabCollection.Tabs.Length > 0)
                    {
                        foreach (var childTab in tabCollection.Tabs)
                        {
                            if (!TabManager.IsValid(childTab, PermissionList)) continue;

                            //var childUrl = PageUtils.AddQueryString(childTab.Href, new NameValueCollection { { "siteId", SiteId.ToString() } });
                            //if (!string.IsNullOrEmpty(childUrl) && !StringUtils.EqualsIgnoreCase(childUrl, PageUtils.UnclickedUrl))
                            //{
                            //    childUrl = PageUtils.GetLoadingUrl(childUrl);
                            //}
                            var href = childTab.Href;
                            if (!PageUtils.IsAbsoluteUrl(href))
                            {
                                href = PageUtils.AddQueryString(href,
                                    new NameValueCollection { { "siteId", SiteId.ToString() } });
                            }
                            href = childTab.HasHref ? PageUtils.GetLoadingUrl(href) : "javascript:;";

                            childBuilder.Append($@"
<li>
    <a href=""{href}"" target=""{(string.IsNullOrEmpty(childTab.Target) ? "right" : childTab.Target)}"">
        <i class=""{childTab.IconClass}""></i>
        {childTab.Text}
    </a>
</li>");
                        }
                    }
                }

                if (childBuilder.Length > 0)
                {
                    builder.Append($@"
<li class=""has_sub"">
    <a href=""javascript:;"" class=""waves-effect waves-primary {(parent.Selected ? "subdrop" : "")}"" target=""{(string.IsNullOrEmpty(parent.Target) ? "right" : parent.Target)}"">
        <i class=""{parent.IconClass ?? "ion-star"}""></i>
        <span> {parent.Text} </span>
        <span class=""menu-arrow""></span>
    </a>
    <ul class=""list-unstyled"" style=""display: {(parent.Selected ? "block" : "none")};"">
        {childBuilder}
    </ul>
</li>
");
                }
                else
                {
                    var href = parent.Href;
                    if (!PageUtils.IsAbsoluteUrl(href))
                    {
                        href = PageUtils.AddQueryString(href,
                            new NameValueCollection {{"siteId", SiteId.ToString()}});
                    }
                    href = parent.HasHref ? PageUtils.GetLoadingUrl(href) : "javascript:;";
                    builder.Append($@"
<li class=""has_sub"">
    <a href=""{href}"" class=""waves-effect waves-primary {(parent.Selected ? "subdrop" : "")}"" target=""{(string.IsNullOrEmpty(parent.Target) ? "right" : parent.Target)}"">
        <i class=""{parent.IconClass ?? "ion-star"}""></i>
        <span> {parent.Text} </span>
    </a>
</li>");
                }
            }
        }

        /// <summary>
        /// Creates the markup for the current TabCollection
        /// </summary>
        /// <returns></returns>
        //protected void BuildNavigationTree(StringBuilder builder, List<Tab> tabs, int parentsCount, bool isDisplay)
        //{
        //    if (tabs == null) return;

        //    foreach (var parent in tabs)
        //    {
        //        if (!TabManager.IsValid(parent, PermissionList)) continue;

        //        var linkUrl = FormatLink(parent);
        //        //if (!string.IsNullOrEmpty(linkUrl) && !StringUtils.EqualsIgnoreCase(linkUrl, PageUtils.UnclickedUrl))
        //        //{
        //        //    linkUrl = PageUtils.GetLoadingUrl(linkUrl);
        //        //}
        //        var hasChildren = parent.Children != null && parent.Children.Length > 0;
        //        var openWindow = !hasChildren && StringUtils.EndsWithIgnoreCase(parent.Href, "main.aspx");

        //        var item = NavigationTreeItem.CreateNavigationBarItem(isDisplay, parent.Selected, parentsCount, hasChildren, openWindow, parent.Text, linkUrl, parent.Target, parent.Enabled, parent.IconUrl);

        //        builder.Append(item.GetTrHtml());

        //        if (SiteId > 0)
        //        {
        //            var tabCollection = NodeNaviTabManager.GetTabCollection(parent, SiteId);
        //            if (tabCollection?.Tabs != null && tabCollection.Tabs.Length > 0)
        //            {
        //                BuildNavigationTree(builder, tabCollection.Tabs.ToList(), parentsCount + 1, parent.Selected);
        //            }
        //        }
        //        else
        //        {
        //            if (parent.Children != null && parent.Children.Length > 0)
        //            {
        //                BuildNavigationTree(builder, parent.Children.ToList(), parentsCount + 1, parent.Selected);
        //            }
        //        }
        //    }
        //}

        public int SiteId
        {
            get
            {
                var state = ViewState["SiteId"];
                if (state == null)
                {
                    return 0;
                }
                return (int)state;
            }
            set
            {
                ViewState["SiteId"] = value;
            }
        }

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

        public string Title
        {
            get
            {
                var state = ViewState["Title"];
                if (state == null)
                {
                    return string.Empty;
                }
                return (string)state;
            }
            set
            {
                ViewState["Title"] = value;
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

        protected virtual string GetText(Tab t)
        {
            return t.Text;
        }
    }
}
