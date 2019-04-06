using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using System.Linq;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.BackgroundPages
{
    public class PageMain : BasePageCms
    {
        protected override bool IsSinglePage => true;

        public string InnerApiUrl => ApiManager.InnerApiUrl.TrimEnd('/');

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSiteServerUrl(nameof(PageMain), null);
        }

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetSiteServerUrl(nameof(PageMain), new NameValueCollection
            {
                {"siteId", siteId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var isLeft = false;
            var adminInfo = AuthRequest.AdminInfo;
            var permissions = AuthRequest.AdminPermissionsImpl;

            var pageSiteId = SiteId;
            var currentSiteId = 0;
            var siteIdList = permissions.GetSiteIdList();
            var siteInfoList = new List<SiteInfo>();
            SiteInfo rootSiteInfo = null;
            foreach (var theSiteId in siteIdList)
            {
                var siteInfo = SiteManager.GetSiteInfo(theSiteId);
                if (siteInfo == null) continue;
                if (siteInfo.IsRoot)
                {
                    rootSiteInfo = siteInfo;
                }
                siteInfoList.Add(siteInfo);
            }

            if (siteIdList.Contains(pageSiteId))
            {
                currentSiteId = pageSiteId;
            }
            else if (siteIdList.Contains(adminInfo.SiteId))
            {
                currentSiteId = adminInfo.SiteId;
            }

            if (currentSiteId == 0 || !SiteManager.IsExists(currentSiteId) || !siteIdList.Contains(currentSiteId))
            {
                if (siteIdList.Count > 0)
                {
                    currentSiteId = siteIdList[0];
                }
            }

            var currentSiteInfo = SiteManager.GetSiteInfo(currentSiteId);
            var addedSiteIdList = new List<int>();

            if (currentSiteInfo != null && currentSiteInfo.Id > 0)
            {
                if (pageSiteId == 0)
                {
                    PageUtils.Redirect(GetRedirectUrl(currentSiteInfo.Id));
                    return;
                }
            }
            else
            {
                if (permissions.IsConsoleAdministrator)
                {
                    PageUtils.Redirect(PageSiteAdd.GetRedirectUrl());
                    return;
                }
            }

            if (currentSiteInfo != null && currentSiteInfo.Id > 0)
            {
                var permissionList = new List<string>(permissions.PermissionList);

                if (permissions.HasSitePermissions(currentSiteInfo.Id))
                {
                    var websitePermissionList = permissions.GetSitePermissions(currentSiteInfo.Id);
                    if (websitePermissionList != null)
                    {
                        isLeft = true;
                        permissionList.AddRange(websitePermissionList);
                    }
                }

                var channelPermissions = permissions.GetChannelPermissions(currentSiteInfo.Id);
                if (channelPermissions.Count > 0)
                {
                    isLeft = true;
                    permissionList.AddRange(channelPermissions);
                }

                //LtlLeftManagement.Text =
                //    NavigationTree.BuildNavigationTree(currentSiteInfo.Id, ConfigManager.TopMenu.IdSite,
                //        permissionList);

                //LtlLeftFunctions.Text = NavigationTree.BuildNavigationTree(currentSiteInfo.Id, string.Empty,
                //    permissionList);

                if (adminInfo.SiteId != currentSiteInfo.Id)
                {
                    DataProvider.AdministratorDao.UpdateSiteId(adminInfo, currentSiteInfo.Id);
                }
            }

            //LtlTopMenus.Text = isLeft
            //    ? GetTopMenuSitesHtml(permissions, siteInfoList, rootSiteInfo, addedSiteIdList, currentSiteInfo) +
            //      GetTopMenuLinksHtml(currentSiteInfo) + GetTopMenusHtml(permissions, pageSiteId)
            //    : GetTopMenusHtml(permissions, pageSiteId);
        }

        private static void AddSite(StringBuilder builder, SiteInfo siteInfoToAdd, Dictionary<int, List<SiteInfo>> parentWithChildren, int level, List<int> addedSiteIdList, SiteInfo currentSiteInfo)
        {
            if (addedSiteIdList.Contains(siteInfoToAdd.Id)) return;

            var loadingUrl = PageUtils.GetLoadingUrl(GetRedirectUrl(siteInfoToAdd.Id));

            if (parentWithChildren.ContainsKey(siteInfoToAdd.Id))
            {
                var children = parentWithChildren[siteInfoToAdd.Id];

                builder.Append($@"
<li class=""has-submenu {(siteInfoToAdd.Id == currentSiteInfo.Id ? "active" : "")}"">
    <a href=""{loadingUrl}"">{siteInfoToAdd.SiteName}</a>
    <ul class=""submenu"">
");

                level++;

                var list = children.OrderByDescending(o => o.Taxis).ToList();

                foreach (var subSiteInfo in list)
                {
                    AddSite(builder, subSiteInfo, parentWithChildren, level, addedSiteIdList, currentSiteInfo);
                }

                builder.Append(@"
    </ul>
</li>");
            }
            else
            {
                builder.Append(
                    $@"<li class=""{(siteInfoToAdd.Id == currentSiteInfo.Id ? "active" : "")}""><a href=""{loadingUrl}"">{siteInfoToAdd.SiteName}</a></li>");
            }

            addedSiteIdList.Add(siteInfoToAdd.Id);
        }

        private static string GetTopMenuSitesHtml(PermissionsImpl permissions, List<SiteInfo> siteInfoList, SiteInfo rootSiteInfo, List<int> addedSiteIdList, SiteInfo currentSiteInfo)
        {
            if (siteInfoList.Count == 0)
            {
                return string.Empty;
            }

            //操作者拥有的站点列表
            var mySiteInfoList = new List<SiteInfo>();

            var parentWithChildren = new Dictionary<int, List<SiteInfo>>();

            if (permissions.IsSystemAdministrator)
            {
                foreach (var siteInfo in siteInfoList)
                {
                    AddToMySiteInfoList(mySiteInfoList, siteInfo, parentWithChildren);
                }
            }
            else
            {
                var permissionChannelIdList = permissions.ChannelPermissionChannelIdList;
                foreach (var siteInfo in siteInfoList)
                {
                    var showSite = IsShowSite(siteInfo.Id, permissionChannelIdList);
                    if (showSite)
                    {
                        AddToMySiteInfoList(mySiteInfoList, siteInfo, parentWithChildren);
                    }
                }
            }

            var builder = new StringBuilder();

            if (rootSiteInfo != null || mySiteInfoList.Count > 0)
            {
                if (rootSiteInfo != null)
                {
                    AddSite(builder, rootSiteInfo, parentWithChildren, 0, addedSiteIdList, currentSiteInfo);
                }

                if (mySiteInfoList.Count > 0)
                {
                    var count = 0;
                    var list = mySiteInfoList.OrderByDescending(o => o.Taxis).ToList();
                    foreach (var siteInfo in list)
                    {
                        if (siteInfo.IsRoot == false)
                        {
                            count++;
                            AddSite(builder, siteInfo, parentWithChildren, 0, addedSiteIdList, currentSiteInfo);
                        }
                        if (count == 13)
                        {
                            break;
                        }
                    }
                    builder.Append(
                        $@"<li><a href=""javascript:;"" onclick=""{ModalSiteSelect.GetOpenLayerString(currentSiteInfo.Id)}"">全部站点...</a></li>");
                }
            }

            var clazz = "has-submenu";
            var menuText = "站点管理";
            if (currentSiteInfo != null && currentSiteInfo.Id > 0)
            {
                clazz = "has-submenu active";
                menuText = currentSiteInfo.SiteName;
                if (currentSiteInfo.ParentId > 0)
                {
                    menuText += $" ({SiteManager.GetSiteLevel(currentSiteInfo.Id) + 1}级)";
                }
            }

            return $@"<li class=""{clazz}"">
              <a href=""javascript:;"">{menuText}</a>
              <ul class=""submenu"">
                {builder}
              </ul>
            </li>";
        }

        private static string GetTopMenuLinksHtml(SiteInfo currentSiteInfo)
        {
            if (currentSiteInfo == null || currentSiteInfo.Id <= 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            builder.Append(
                    $@"<li><a href=""{PageUtility.GetSiteUrl(currentSiteInfo, false)}"" target=""_blank"">访问站点</a></li>");
            builder.Append(
                    $@"<li><a href=""{ApiRoutePreview.GetSiteUrl(currentSiteInfo.Id)}"" target=""_blank"">预览站点</a></li>");

            return $@"<li class=""has-submenu"">
              <a href=""javascript:;"">站点链接</a>
              <ul class=""submenu"">
                {builder}
              </ul>
            </li>";
        }

        private static string GetTopMenusHtml(PermissionsImpl permissions, int siteId)
        {
            var topMenuTabs = TabManager.GetTopMenuTabs();

            if (topMenuTabs == null || topMenuTabs.Count == 0)
            {
                return string.Empty;
            }

            var permissionList = new List<string>();
            if (permissions.HasSitePermissions(siteId))
            {
                var websitePermissionList = permissions.GetSitePermissions(siteId);
                if (websitePermissionList != null)
                {
                    permissionList.AddRange(websitePermissionList);
                }
            }

            permissionList.AddRange(permissions.PermissionList);

            var builder = new StringBuilder();
            foreach (var tab in topMenuTabs)
            {
                if (!permissions.IsConsoleAdministrator && !TabManager.IsValid(tab, permissionList)) continue;

                var tabs = TabManager.GetTabList(tab.Id, 0);
                var tabsBuilder = new StringBuilder();
                foreach (var parent in tabs)
                {
                    if (!permissions.IsConsoleAdministrator && !TabManager.IsValid(parent, permissionList)) continue;

                    var hasChildren = parent.Children != null && parent.Children.Length > 0;

                    var parentUrl = !string.IsNullOrEmpty(parent.Href) ? PageUtils.GetLoadingUrl(parent.Href) : "javascript:;";
                    var parentTarget = !string.IsNullOrEmpty(parent.Target) ? parent.Target : "right";

                    if (hasChildren)
                    {
                        tabsBuilder.Append($@"
<li class=""has-submenu"">
    <a href=""{parentUrl}"" target=""{parentTarget}"">{parent.Text}</a>
    <ul class=""submenu"">
");

                        if (parent.Children != null && parent.Children.Length > 0)
                        {
                            foreach (var childTab in parent.Children)
                            {
                                var childTarget = !string.IsNullOrEmpty(childTab.Target) ? childTab.Target : "right";
                                tabsBuilder.Append($@"<li><a href=""{PageUtils.GetLoadingUrl(childTab.Href)}"" target=""{childTarget}"">{childTab.Text}</a></li>");
                            }
                        }

                        tabsBuilder.Append(@"
    </ul>
</li>");
                    }
                    else
                    {
                        tabsBuilder.Append(
                            $@"<li><a href=""{parentUrl}"" target=""{parentTarget}"">{parent.Text}</a></li>");
                    }
                }

                var url = !string.IsNullOrEmpty(tab.Href) ? PageUtils.ParseNavigationUrl(tab.Href) : "javascript:;";
                var target = !string.IsNullOrEmpty(tab.Target) ? tab.Target : "right";

                builder.Append(
                    $@"<li class=""has-submenu"">
                        <a href=""{url}"" target=""{target}"">{tab.Text}</a>
                        <ul class=""submenu"">
                            {tabsBuilder}
                        </ul>
                       </li>");
            }

            return builder.ToString();
        }

        private static bool IsShowSite(int siteId, List<int> permissionChannelIdList)
        {
            foreach (var permissionChannelId in permissionChannelIdList)
            {
                if (ChannelManager.IsAncestorOrSelf(siteId, siteId, permissionChannelId))
                {
                    return true;
                }
            }
            return false;
        }

        private static void AddToMySiteInfoList(List<SiteInfo> mySiteInfoList, SiteInfo mySiteInfo, Dictionary<int, List<SiteInfo>> parentWithChildren)
        {
            if (mySiteInfo == null) return;

            if (mySiteInfo.ParentId > 0)
            {
                var children = new List<SiteInfo>();
                if (parentWithChildren.ContainsKey(mySiteInfo.ParentId))
                {
                    children = parentWithChildren[mySiteInfo.ParentId];
                }
                children.Add(mySiteInfo);
                parentWithChildren[mySiteInfo.ParentId] = children;
            }
            mySiteInfoList.Add(mySiteInfo);
        }
    }
}
