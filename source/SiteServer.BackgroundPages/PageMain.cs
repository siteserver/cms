using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Permissions;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages
{
    public class PageMain : BasePageCms
    {
        public Literal LtlTopMenus;
        public Literal LtlExtras;
        public Literal LtlUserName;
        public NavigationTree NtLeftManagement;
        public NavigationTree NtLeftFunctions;

        private PublishmentSystemInfo _publishmentSystemInfo = new PublishmentSystemInfo();
        private PublishmentSystemInfo _hqPublishmentSystemInfo;
        private readonly List<int> _addedSiteIdList = new List<int>();
        private AdministratorWithPermissions _permissions;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSiteServerUrl(nameof(PageMain), null);
        }

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetSiteServerUrl(nameof(PageMain), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            LtlUserName.Text = AdminManager.GetDisplayName(Body.AdministratorName, true);

            _permissions = PermissionsManager.GetPermissions(Body.AdministratorName);

            var publishmentSystemId = PublishmentSystemId;

            if (publishmentSystemId == 0)
            {
                publishmentSystemId = Body.AdministratorInfo.PublishmentSystemId;
            }

            var publishmentSystemIdList = ProductPermissionsManager.Current.PublishmentSystemIdList;

            //站点要判断是否存在，是否有权限
            if (publishmentSystemId == 0 || !PublishmentSystemManager.IsExists(publishmentSystemId) || !publishmentSystemIdList.Contains(publishmentSystemId))
            {
                if (publishmentSystemIdList != null && publishmentSystemIdList.Count > 0)
                {
                    publishmentSystemId = publishmentSystemIdList[0];
                }
            }

            _publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            if (_publishmentSystemInfo != null && _publishmentSystemInfo.PublishmentSystemId > 0)
            {
                if (PublishmentSystemId == 0)
                {
                    PageUtils.Redirect(GetRedirectUrl(_publishmentSystemInfo.PublishmentSystemId));
                    return;
                }

                var showPublishmentSystem = false;

                var permissionList = new List<string>();
                if (ProductPermissionsManager.Current.WebsitePermissionDict.ContainsKey(_publishmentSystemInfo.PublishmentSystemId))
                {
                    var websitePermissionList = ProductPermissionsManager.Current.WebsitePermissionDict[_publishmentSystemInfo.PublishmentSystemId];
                    if (websitePermissionList != null)
                    {
                        showPublishmentSystem = true;
                        permissionList.AddRange(websitePermissionList);
                    }
                }

                ICollection nodeIdCollection = ProductPermissionsManager.Current.ChannelPermissionDict.Keys;
                foreach (int nodeId in nodeIdCollection)
                {
                    if (NodeManager.IsAncestorOrSelf(_publishmentSystemInfo.PublishmentSystemId, _publishmentSystemInfo.PublishmentSystemId, nodeId))
                    {
                        showPublishmentSystem = true;
                        var list = ProductPermissionsManager.Current.ChannelPermissionDict[nodeId];
                        permissionList.AddRange(list);
                    }
                }

                var publishmentSystemIdHashtable = new Hashtable();
                if (publishmentSystemIdList != null)
                {
                    foreach (var thePublishmentSystemId in publishmentSystemIdList)
                    {
                        publishmentSystemIdHashtable.Add(thePublishmentSystemId, thePublishmentSystemId);
                    }
                }

                if (!publishmentSystemIdHashtable.Contains(PublishmentSystemId))
                {
                    showPublishmentSystem = false;
                }

                if (!showPublishmentSystem)
                {
                    PageUtils.RedirectToErrorPage("您没有此发布系统的操作权限！");
                    return;
                }

                LtlTopMenus.Text = GetTopMenuSitesHtml();
                LtlTopMenus.Text += GetTopMenuLinksHtml();
                if (_permissions.IsConsoleAdministrator || _permissions.PermissionList.Count > 0)
                {
                    LtlTopMenus.Text += GetTopMenusHtml();
                }

                LtlExtras.Text = $@"
<li>
    <form role=""search"" class=""navbar-left app-search pull-left hidden-xs"" action=""{PageContentSearch.GetRedirectUrl(_publishmentSystemInfo.PublishmentSystemId)}"" target=""right"" method=""get"">
        <input name=""Keyword"" type=""text"" placeholder=""内容搜索..."" class=""form-control"">
        <a href=""javascript:;"" onclick=""""><i class=""ion-search""></i></a>
    </form>
</li>
<li class=""dropdown hidden-xs"">
    <a href=""{PageCreateStatus.GetRedirectUrl(_publishmentSystemInfo.PublishmentSystemId)}"" target=""right"" class=""waves-effect waves-light"">
        <i class=""ion-wand""></i>
        <span id=""progress"" class=""badge badge-xs badge-pink"">0</span>
    </a>
</li>
";

                NtLeftManagement.TopId = AppManager.IdSite;
                NtLeftManagement.PublishmentSystemId = _publishmentSystemInfo.PublishmentSystemId;
                NtLeftManagement.PermissionList = permissionList;

                NtLeftFunctions.TopId = string.Empty;
                NtLeftFunctions.PublishmentSystemId = _publishmentSystemInfo.PublishmentSystemId;
                NtLeftFunctions.PermissionList = permissionList;

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", NodeNaviTreeItem.GetNavigationBarScript());
            }
            else
            {
                if (_permissions.IsSystemAdministrator)
                {
                    PageUtils.Redirect(PagePublishmentSystemAdd.GetRedirectUrl());
                    return;
                }
            }

            if (_publishmentSystemInfo != null && _publishmentSystemInfo.PublishmentSystemId > 0 && Body.AdministratorInfo.PublishmentSystemId != _publishmentSystemInfo.PublishmentSystemId)
            {
                BaiRongDataProvider.AdministratorDao.UpdatePublishmentSystemId(Body.AdministratorName, _publishmentSystemInfo.PublishmentSystemId);
            }
        }

        private void AddSite(StringBuilder builder, PublishmentSystemInfo publishmentSystemInfo, Dictionary<int, List<PublishmentSystemInfo>> parentWithChildren, int level)
        {
            if (_addedSiteIdList.Contains(publishmentSystemInfo.PublishmentSystemId)) return;

            var loadingUrl = PageUtils.GetLoadingUrl(GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId));

            if (parentWithChildren.ContainsKey(publishmentSystemInfo.PublishmentSystemId))
            {
                var children = parentWithChildren[publishmentSystemInfo.PublishmentSystemId];

                builder.Append($@"
<li class=""has-submenu {(publishmentSystemInfo.PublishmentSystemId == _publishmentSystemInfo.PublishmentSystemId ? "active" : "")}"">
    <a href=""{loadingUrl}"">{publishmentSystemInfo.PublishmentSystemName}</a>
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
                    $@"<li class=""{(publishmentSystemInfo.PublishmentSystemId == _publishmentSystemInfo.PublishmentSystemId ? "active" : "")}""><a href=""{loadingUrl}"">{publishmentSystemInfo.PublishmentSystemName}</a></li>");
            }

            _addedSiteIdList.Add(publishmentSystemInfo.PublishmentSystemId);
        }

        private string GetTopMenuSitesHtml()
        {
            var publishmentSystemIdList = ProductPermissionsManager.Current.PublishmentSystemIdList;

            if (!_permissions.IsSystemAdministrator && publishmentSystemIdList.Count == 0)
            {
                return string.Empty;
            }

            //操作者拥有的站点列表
            var mySystemInfoList = new List<PublishmentSystemInfo>();

            var parentWithChildren = new Dictionary<int, List<PublishmentSystemInfo>>();

            if (ProductPermissionsManager.Current.IsSystemAdministrator)
            {
                foreach (var publishmentSystemId in publishmentSystemIdList)
                {
                    AddToMySystemInfoList(mySystemInfoList, parentWithChildren, publishmentSystemId);
                }
            }
            else
            {
                ICollection nodeIdCollection = ProductPermissionsManager.Current.ChannelPermissionDict.Keys;
                ICollection publishmentSystemIdCollection = ProductPermissionsManager.Current.WebsitePermissionDict.Keys;
                foreach (var publishmentSystemId in publishmentSystemIdList)
                {
                    var showPublishmentSystem = IsShowPublishmentSystem(publishmentSystemId, publishmentSystemIdCollection, nodeIdCollection);
                    if (showPublishmentSystem)
                    {
                        AddToMySystemInfoList(mySystemInfoList, parentWithChildren, publishmentSystemId);
                    }
                }
            }

            var builder = new StringBuilder();

            if (_hqPublishmentSystemInfo != null || mySystemInfoList.Count > 0)
            {
                if (_hqPublishmentSystemInfo != null)
                {
                    AddSite(builder, _hqPublishmentSystemInfo, parentWithChildren, 0);
                }

                if (mySystemInfoList.Count > 0)
                {
                    var count = 0;
                    foreach (var publishmentSystemInfo in mySystemInfoList)
                    {
                        if (publishmentSystemInfo.IsHeadquarters == false)
                        {
                            count++;
                            AddSite(builder, publishmentSystemInfo, parentWithChildren, 0);
                        }
                        if (count == 13)
                        {
                            builder.Append(
                                $@"<li><a href=""javascript:;"" onclick=""{ModalPublishmentSystemSelect.GetOpenLayerString()}"">列出全部站点...</a></li>");
                            break;
                        }
                    }
                }
            }

            var clazz = "has-submenu";
            var menuText = "站点管理";
            if (_publishmentSystemInfo != null && _publishmentSystemInfo.PublishmentSystemId > 0)
            {
                clazz = "has-submenu active";
                menuText = _publishmentSystemInfo.PublishmentSystemName;
                if (_publishmentSystemInfo.ParentPublishmentSystemId > 0)
                {
                    menuText += $" ({PublishmentSystemManager.GetPublishmentSystemLevel(_publishmentSystemInfo.PublishmentSystemId) + 1}级)";
                }
            }

            return $@"<li class=""{clazz}"">
              <a href=""javascript:;""><i class=""ion-earth""></i>{menuText}</a>
              <ul class=""submenu"">
                {builder}
              </ul>
            </li>";
        }

        private string GetTopMenuLinksHtml()
        {
            if (_publishmentSystemInfo == null || _publishmentSystemInfo.PublishmentSystemId <= 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();

            if (_publishmentSystemInfo.Additional.IsMultiDeployment)
            {
                if (!string.IsNullOrEmpty(_publishmentSystemInfo.Additional.OuterSiteUrl))
                {
                    builder.Append(
                    $@"<li><a href=""{_publishmentSystemInfo.Additional.OuterSiteUrl}"" target=""_blank""><i class=""icon-external-link""></i> 进入站点外网地址</a></li>");
                }
                if (!string.IsNullOrEmpty(_publishmentSystemInfo.Additional.InnerSiteUrl))
                {
                    builder.Append(
                        $@"<li><a href=""{_publishmentSystemInfo.Additional.InnerSiteUrl}"" target=""_blank""><i class=""icon-external-link""></i> 进入站点内网地址</a></li>");
                }
            }
            else
            {
                var publishmentSystemUrl = PageUtility.GetPublishmentSystemUrl(_publishmentSystemInfo, string.Empty, true);
                builder.Append(
                    $@"<li><a href=""{publishmentSystemUrl}"" target=""_blank""><i class=""icon-external-link""></i> 进入站点</a></li>");
            }

            builder.Append(
                    $@"<li><a href=""{HomeUtils.GetUrl(_publishmentSystemInfo.Additional.HomeUrl, string.Empty)}"" target=""_blank""><i class=""icon-external-link""></i> 进入用户中心</a></li>");

            return $@"<li class=""has-submenu"">
              <a href=""javascript:;""><i class=""ion-link""></i>站点访问地址</a>
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
            if (ProductPermissionsManager.Current.WebsitePermissionDict.ContainsKey(PublishmentSystemId))
            {
                var websitePermissionList = ProductPermissionsManager.Current.WebsitePermissionDict[PublishmentSystemId];
                if (websitePermissionList != null)
                {
                    permissionList.AddRange(websitePermissionList);
                }
            }

            permissionList.AddRange(_permissions.PermissionList);

            var builder = new StringBuilder();
            foreach (var tab in topMenuTabs)
            {
                if (!TabManager.IsValid(tab, _permissions.PermissionList)) continue;

                var target = string.Empty;
                if (!string.IsNullOrEmpty(tab.Target))
                {
                    target = tab.Target;
                }

                var tabs = TabManager.GetTabList(tab.Id, 0);
                var tabsBuilder = new StringBuilder();
                foreach (var parent in tabs)
                {
                    if (!TabManager.IsValid(parent, permissionList)) continue;

                    var hasChildren = parent.Children != null && parent.Children.Length > 0;

                    var url = parent.HasHref ? PageUtils.GetLoadingUrl(parent.Href) : "javascript:;";

                    if (hasChildren)
                    {
                        tabsBuilder.Append($@"
<li class=""has-submenu"">
    <a href=""{url}"" target=""right"">{parent.Text}</a>
    <ul class=""submenu"">
");

                        if (parent.Children != null && parent.Children.Length > 0)
                        {
                            foreach (var childTab in parent.Children)
                            {
                                tabsBuilder.Append($@"<li><a href=""{PageUtils.GetLoadingUrl(childTab.Href)}"" target=""right"">{childTab.Text}</a></li>");
                            }
                        }

                        tabsBuilder.Append(@"
    </ul>
</li>");
                    }
                    else
                    {
                        tabsBuilder.Append(
                            $@"<li><a href=""{url}"" target=""right"">{parent.Text}</a></li>");
                    }
                }

                builder.Append(
                    $@"<li class=""has-submenu"">
                        <a href=""{(!string.IsNullOrEmpty(tab.Href) ? PageUtils.ParseNavigationUrl(tab.Href) : "javascript:;")}"" target=""{target}""><i class=""{tab.IconClass ?? "ion-ios-more"}""></i>{tab.Text}</a>
                        <ul class=""submenu"">
                            {tabsBuilder}
                        </ul>
                       </li>");
            }

            return builder.ToString();
        }

        private static bool IsShowPublishmentSystem(int publishmentSystemId, ICollection publishmentSystemIdCollection, ICollection nodeIdCollection)
        {
            foreach (int psId in publishmentSystemIdCollection)
            {
                if (psId == publishmentSystemId)
                {
                    return true;
                }
            }
            foreach (int nodeId in nodeIdCollection)
            {
                if (NodeManager.IsAncestorOrSelf(publishmentSystemId, publishmentSystemId, nodeId))
                {
                    return true;
                }
            }
            return false;
        }

        private void AddToMySystemInfoList(List<PublishmentSystemInfo> mySystemInfoList, Dictionary<int, List<PublishmentSystemInfo>> parentWithChildren, int publishmentSystemId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            if (publishmentSystemInfo == null) return;

            if (publishmentSystemInfo.IsHeadquarters)
            {
                _hqPublishmentSystemInfo = publishmentSystemInfo;
            }
            else if (publishmentSystemInfo.ParentPublishmentSystemId > 0)
            {
                var children = new List<PublishmentSystemInfo>();
                if (parentWithChildren.ContainsKey(publishmentSystemInfo.ParentPublishmentSystemId))
                {
                    children = parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId];
                }
                children.Add(publishmentSystemInfo);
                parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId] = children;
            }
            mySystemInfoList.Add(publishmentSystemInfo);
        }
    }
}
