using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Api.Sys.Packaging;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin;

namespace SiteServer.BackgroundPages
{
    public class PageMain : BasePageCms
    {
        public Literal LtlTopMenus;
        public PlaceHolder PhSite;
        public Literal LtlCreateStatus;
        public NavigationTree NtLeftManagement;
        public NavigationTree NtLeftFunctions;

        private SiteInfo _siteInfo = new SiteInfo();
        private SiteInfo _hqSiteInfo;
        private readonly List<int> _addedSiteIdList = new List<int>();

        protected override bool IsSinglePage => true;

        public string SignalrHubsUrl = ApiManager.SignalrHubsUrl;

        public string PackageIdSsCms = PackageUtils.PackageIdSsCms;

        public string PackageList
        {
            get
            {
                var list = new List<object>();
                var dict = PluginManager.GetPluginIdAndVersionDict();
                foreach (var id in dict.Keys)
                {
                    var version = dict[id];
                    list.Add(new
                    {
                        id,
                        version
                    });
                }
                return TranslateUtils.JsonSerialize(list);
            }
        }

        public string DownloadApiUrl => ApiRouteDownload.GetUrl(ApiManager.InnerApiUrl);

        public string CurrentVersion => SystemManager.Version;

        public string UpdateSystemUrl => PageUpdateSystem.GetRedirectUrl();

        public bool IsConsoleAdministrator => AuthRequest.AdminPermissions.IsConsoleAdministrator;

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

            var siteId = 0;

            var siteIdList = AuthRequest.AdminPermissions.SiteIdList;
            if (siteIdList.Contains(SiteId))
            {
                siteId = SiteId;
            }
            else if (siteIdList.Contains(AuthRequest.AdminInfo.SiteId))
            {
                siteId = AuthRequest.AdminInfo.SiteId;
            }

            //站点要判断是否存在，是否有权限
            if (siteId == 0 || !SiteManager.IsExists(siteId) || !siteIdList.Contains(siteId))
            {
                if (siteIdList.Count > 0)
                {
                    siteId = siteIdList[0];
                }
            }

            _siteInfo = SiteManager.GetSiteInfo(siteId);

            if (_siteInfo != null && _siteInfo.Id > 0)
            {
                if (SiteId == 0)
                {
                    PageUtils.Redirect(GetRedirectUrl(_siteInfo.Id));
                    return;
                }

                var permissionList = new List<string>(AuthRequest.AdminPermissions.PermissionList);

                if (AuthRequest.AdminPermissions.HasSitePermissions(_siteInfo.Id))
                {
                    var websitePermissionList = AuthRequest.AdminPermissions.GetSitePermissions(_siteInfo.Id);
                    if (websitePermissionList != null)
                    {
                        isLeft = true;
                        permissionList.AddRange(websitePermissionList);
                    }
                }

                var channelPermissions = AuthRequest.AdminPermissions.GetChannelPermissions(_siteInfo.Id);
                if (channelPermissions.Count > 0)
                {
                    isLeft = true;
                    permissionList.AddRange(channelPermissions);
                }

                PhSite.Visible = isLeft;

                LtlCreateStatus.Text = $@"
<script type=""text/javascript"">
function {LayerUtils.OpenPageCreateStatusFuncName}() {{
    {PageCreateStatus.GetOpenLayerString(_siteInfo.Id)}
}}
</script>
<a href=""javascript:;"" onclick=""{LayerUtils.OpenPageCreateStatusFuncName}()"">
    <i class=""ion-wand""></i>
    <span id=""progress"" class=""badge badge-xs badge-pink"">0</span>
</a>
";

                NtLeftManagement.TopId = ConfigManager.TopMenu.IdSite;
                NtLeftManagement.SiteId = _siteInfo.Id;
                NtLeftManagement.PermissionList = permissionList;

                NtLeftFunctions.TopId = string.Empty;
                NtLeftFunctions.SiteId = _siteInfo.Id;
                NtLeftFunctions.PermissionList = permissionList;

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", NodeNaviTreeItem.GetNavigationBarScript());
            }
            else
            {
                if (IsConsoleAdministrator)
                {
                    PageUtils.Redirect(PageSiteAdd.GetRedirectUrl());
                    return;
                }
            }

            if (_siteInfo != null && _siteInfo.Id > 0 && AuthRequest.AdminInfo.SiteId != _siteInfo.Id)
            {
                DataProvider.AdministratorDao.UpdateSiteId(AuthRequest.AdminName, _siteInfo.Id);
            }

            if (isLeft)
            {
                LtlTopMenus.Text = $@"
<a href=""javascript:;"" class=""position-fixed"" onclick=""toggleMenu()"" style=""margin-top: 10px;margin-left: 30px;"">
    <i class=""ion-navicon"" style=""font-size: 28px;color: #fff;""></i>
</a>
<ul id=""topMenus"" class=""navigation-menu"">
    {GetTopMenuSitesHtml() + GetTopMenuLinksHtml() + GetTopMenusHtml()}
</ul>
";
            }
            else
            {
                LtlTopMenus.Text = $@"
<script>toggleMenu();</script>
<ul id=""topMenus"" class=""navigation-menu"" style=""margin-left: 210px;"">
    {GetTopMenusHtml()}
</ul>
";
            }
        }

        private void AddSite(StringBuilder builder, SiteInfo siteInfo, Dictionary<int, List<SiteInfo>> parentWithChildren, int level)
        {
            if (_addedSiteIdList.Contains(siteInfo.Id)) return;

            var loadingUrl = PageUtils.GetLoadingUrl(GetRedirectUrl(siteInfo.Id));

            if (parentWithChildren.ContainsKey(siteInfo.Id))
            {
                var children = parentWithChildren[siteInfo.Id];

                builder.Append($@"
<li class=""has-submenu {(siteInfo.Id == _siteInfo.Id ? "active" : "")}"">
    <a href=""{loadingUrl}"">{siteInfo.SiteName}</a>
    <ul class=""submenu"">
");

                level++;
                foreach (var subSiteInfo in children)
                {
                    AddSite(builder, subSiteInfo, parentWithChildren, level);
                }

                builder.Append(@"
    </ul>
</li>");
            }
            else
            {
                builder.Append(
                    $@"<li class=""{(siteInfo.Id == _siteInfo.Id ? "active" : "")}""><a href=""{loadingUrl}"">{siteInfo.SiteName}</a></li>");
            }

            _addedSiteIdList.Add(siteInfo.Id);
        }

        private string GetTopMenuSitesHtml()
        {
            var siteIdList = AuthRequest.AdminPermissions.SiteIdList;

            if (siteIdList.Count == 0)
            {
                return string.Empty;
            }

            //操作者拥有的站点列表
            var mySystemInfoList = new List<SiteInfo>();

            var parentWithChildren = new Dictionary<int, List<SiteInfo>>();

            if (AuthRequest.AdminPermissions.IsSystemAdministrator)
            {
                foreach (var siteId in siteIdList)
                {
                    AddToMySystemInfoList(mySystemInfoList, parentWithChildren, siteId);
                }
            }
            else
            {
                var permissionSiteIdList = AuthRequest.AdminPermissions.SiteIdList;
                var permissionChannelIdList = AuthRequest.AdminPermissions.ChannelPermissionChannelIdList;
                foreach (var siteId in siteIdList)
                {
                    var showSite = IsShowSite(siteId, permissionSiteIdList, permissionChannelIdList);
                    if (showSite)
                    {
                        AddToMySystemInfoList(mySystemInfoList, parentWithChildren, siteId);
                    }
                }
            }

            var builder = new StringBuilder();

            if (_hqSiteInfo != null || mySystemInfoList.Count > 0)
            {
                if (_hqSiteInfo != null)
                {
                    AddSite(builder, _hqSiteInfo, parentWithChildren, 0);
                }

                if (mySystemInfoList.Count > 0)
                {
                    var count = 0;
                    foreach (var siteInfo in mySystemInfoList)
                    {
                        if (siteInfo.IsRoot == false)
                        {
                            count++;
                            AddSite(builder, siteInfo, parentWithChildren, 0);
                        }
                        if (count == 13)
                        {
                            builder.Append(
                                $@"<li><a href=""javascript:;"" onclick=""{ModalSiteSelect.GetOpenLayerString(SiteId)}"">列出全部站点...</a></li>");
                            break;
                        }
                    }
                }
            }

            var clazz = "has-submenu";
            var menuText = "站点管理";
            if (_siteInfo != null && _siteInfo.Id > 0)
            {
                clazz = "has-submenu active";
                menuText = _siteInfo.SiteName;
                if (_siteInfo.ParentId > 0)
                {
                    menuText += $" ({SiteManager.GetSiteLevel(_siteInfo.Id) + 1}级)";
                }
            }

            return $@"<li class=""{clazz}"">
              <a href=""javascript:;"">{menuText}</a>
              <ul class=""submenu"">
                {builder}
              </ul>
            </li>";
        }

        private string GetTopMenuLinksHtml()
        {
            if (_siteInfo == null || _siteInfo.Id <= 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            builder.Append(
                    $@"<li><a href=""{PageUtility.GetSiteUrl(_siteInfo, false)}"" target=""_blank"">访问站点</a></li>");
            builder.Append(
                    $@"<li><a href=""{ApiRoutePreview.GetSiteUrl(_siteInfo.Id)}"" target=""_blank"">预览站点</a></li>");

            return $@"<li class=""has-submenu"">
              <a href=""javascript:;"">站点链接</a>
              <ul class=""submenu"">
                {builder}
              </ul>
            </li>";
        }

        private string GetTopMenusHtml()
        {
            var topMenuTabs = TabManager.GetTopMenuTabs();

            if (topMenuTabs == null || topMenuTabs.Count == 0)
            {
                return string.Empty;
            }

            var permissionList = new List<string>();
            if (AuthRequest.AdminPermissions.HasSitePermissions(SiteId))
            {
                var websitePermissionList = AuthRequest.AdminPermissions.GetSitePermissions(SiteId);
                if (websitePermissionList != null)
                {
                    permissionList.AddRange(websitePermissionList);
                }
            }

            permissionList.AddRange(AuthRequest.AdminPermissions.PermissionList);

            var builder = new StringBuilder();
            foreach (var tab in topMenuTabs)
            {
                if (!IsConsoleAdministrator && !TabManager.IsValid(tab, permissionList)) continue;

                if (tab.Id == "Account")
                {
                    tab.Text = AuthRequest.AdminName;
                }

                var tabs = TabManager.GetTabList(tab.Id, 0);
                var tabsBuilder = new StringBuilder();
                foreach (var parent in tabs)
                {
                    if (!IsConsoleAdministrator && !TabManager.IsValid(parent, permissionList)) continue;

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

        private static bool IsShowSite(int siteId, List<int> permissionSiteIdList, List<int> permissionChannelIdList)
        {
            foreach (var permissionSiteId in permissionSiteIdList)
            {
                if (permissionSiteId == siteId)
                {
                    return true;
                }
            }
            foreach (var permissionChannelId in permissionChannelIdList)
            {
                if (ChannelManager.IsAncestorOrSelf(siteId, siteId, permissionChannelId))
                {
                    return true;
                }
            }
            return false;
        }

        private void AddToMySystemInfoList(List<SiteInfo> mySystemInfoList, Dictionary<int, List<SiteInfo>> parentWithChildren, int siteId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            if (siteInfo == null) return;

            if (siteInfo.IsRoot)
            {
                _hqSiteInfo = siteInfo;
            }
            else if (siteInfo.ParentId > 0)
            {
                var children = new List<SiteInfo>();
                if (parentWithChildren.ContainsKey(siteInfo.ParentId))
                {
                    children = parentWithChildren[siteInfo.ParentId];
                }
                children.Add(siteInfo);
                parentWithChildren[siteInfo.ParentId] = children;
            }
            mySystemInfoList.Add(siteInfo);
        }
    }
}
