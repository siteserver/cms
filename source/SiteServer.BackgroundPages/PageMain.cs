using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;
using BaiRong.Core.Tabs;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.BackgroundPages.Sys;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages
{
    public class PageMain : BasePageCms
    {
        public NodeNaviTree NtLeftMenuSite;
        public NavigationTree NtLeftMenuSystem;
        public Repeater RptTopMenu;
        public Literal LtlUserName;

        private string _menuId = string.Empty;
        private PublishmentSystemInfo _publishmentSystemInfo = new PublishmentSystemInfo();
        private PublishmentSystemInfo _hqPublishmentSystemInfo;
        private readonly ArrayList _addedSiteIdArrayList = new ArrayList();
        private AdministratorWithPermissions _permissions;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSiteServerUrl(nameof(PageMain), null);
        }

        public static string GetRedirectUrl(int publishmentSystemId, string menuId)
        {
            if (publishmentSystemId > 0)
            {
                return PageUtils.GetSiteServerUrl(nameof(PageMain), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()}
                });
            }
            if (!string.IsNullOrEmpty(menuId))
            {
                return PageUtils.GetSiteServerUrl(nameof(PageMain), new NameValueCollection
                {
                    {"menuID", menuId}
                });
            }
            return PageUtils.GetSiteServerUrl(nameof(PageMain), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            LtlUserName.Text = AdminManager.GetDisplayName(Body.AdministratorName, true);

            _menuId = Body.GetQueryString("menuID");
            _permissions = PermissionsManager.GetPermissions(Body.AdministratorName);

            if (string.IsNullOrEmpty(_menuId))
            {
                var publishmentSystemId = PublishmentSystemId;

                if (publishmentSystemId == 0)
                {
                    publishmentSystemId = Body.AdministratorInfo.PublishmentSystemId;
                }

                var publishmentSystemIdList = ProductPermissionsManager.Current.PublishmentSystemIdList;

                //站点要判断是否存在，是否有权限，update by sessionliang 20160104
                if (publishmentSystemId == 0 || !PublishmentSystemManager.IsExists(publishmentSystemId) || !publishmentSystemIdList.Contains(publishmentSystemId))
                {
                    //ArrayList publishmentSystemIDArrayList = PublishmentSystemManager.GetPublishmentSystemIDArrayListOrderByLevel();
                    // List<int> publishmentSystemIDList = ProductPermissionsManager.Current.PublishmentSystemIDList;
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
                        PageUtils.Redirect(GetRedirectUrl(_publishmentSystemInfo.PublishmentSystemId, string.Empty));
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

                    var appId = EPublishmentSystemTypeUtils.GetValue(_publishmentSystemInfo.PublishmentSystemType);

                    NtLeftMenuSite.FileName = $"~/SiteFiles/Configuration/Menus/{appId}/Management.config";
                    NtLeftMenuSite.PublishmentSystemId = _publishmentSystemInfo.PublishmentSystemId;
                    NtLeftMenuSite.PermissionList = permissionList;

                    ClientScriptRegisterClientScriptBlock("NodeTreeScript", NodeNaviTreeItem.GetNavigationBarScript());
                }
                else
                {
                    if (_permissions.IsSystemAdministrator)
                    {
                        PageUtils.Redirect(PageAppAdd.GetRedirectUrl());
                        return;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(_menuId))
            {
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
                NtLeftMenuSystem.FileName = $"~/SiteFiles/Configuration/Menus/{_menuId}.config";
                NtLeftMenuSystem.PermissionList = permissionList;

                ClientScriptRegisterClientScriptBlock("NodeTreeScript", NavigationTreeItem.GetNavigationBarScript());
            }

            var topMenuList = new List<int> {1, 2};
            //cms超管和有权限的管理员
            if (_permissions.IsConsoleAdministrator || _permissions.PermissionList.Count > 0)
            {
                topMenuList.Add(3);
            }
            RptTopMenu.DataSource = topMenuList;
            RptTopMenu.ItemDataBound += RptTopMenu_ItemDataBound;
            RptTopMenu.DataBind();

            //update at 20141106，避免空引用异常
            if (_publishmentSystemInfo != null && _publishmentSystemInfo.PublishmentSystemId > 0)
            {
                BaiRongDataProvider.AdministratorDao.UpdatePublishmentSystemId(Body.AdministratorName, _publishmentSystemInfo.PublishmentSystemId);
            }
        }

        private void RptTopMenu_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var ltlMenuLi = (Literal)e.Item.FindControl("ltlMenuLi");
            var ltlMenuName = (Literal)e.Item.FindControl("ltlMenuName");
            var ltlMenues = (Literal)e.Item.FindControl("ltlMenues");
            var index = e.Item.ItemIndex;

            if (index == 0)
            {
                if (_publishmentSystemInfo != null && _publishmentSystemInfo.PublishmentSystemId > 0)
                {
                    ltlMenuLi.Text = @"<li class=""active"">";
                    ltlMenuName.Text =
                        $@"{EPublishmentSystemTypeUtils.GetIconHtml(_publishmentSystemInfo.PublishmentSystemType)}&nbsp;{_publishmentSystemInfo
                            .PublishmentSystemName}";
                }
                else
                {
                    ltlMenuLi.Text = @"<li>";
                    ltlMenuName.Text = "站点管理";
                }

                var builder = new StringBuilder();

                builder.Append(@"<ul class=""dropdown-menu"">");

                var isPublishmentSystemList = GetPublishmentSystemListHtml(builder);

                builder.Append(@"</ul>");

                if (isPublishmentSystemList)
                {
                    ltlMenues.Text = builder.ToString();
                }
                else
                {
                    e.Item.Visible = false;
                }
            }
            else if (index == 1)
            {
                if (_publishmentSystemInfo != null && _publishmentSystemInfo.PublishmentSystemId > 0)
                {
                    ltlMenuLi.Text = @"<li>";
                    ltlMenuName.Text = "站点访问地址";

                    var builder = new StringBuilder();

                    builder.Append(@"<ul class=""dropdown-menu"">");

                    if (_publishmentSystemInfo.Additional.IsMultiDeployment)
                    {
                        builder.Append(
                            $@"<li><a href=""{_publishmentSystemInfo.Additional.OuterUrl}"" target=""_blank""><i class=""icon-external-link""></i> 进入站点外网地址</a></li>");
                        builder.Append(
                            $@"<li><a href=""{_publishmentSystemInfo.Additional.InnerUrl}"" target=""_blank""><i class=""icon-external-link""></i> 进入站点内网地址</a></li>");
                    }
                    else
                    {
                        var publishmentSystemUrl = PageUtility.GetPublishmentSystemUrl(_publishmentSystemInfo, string.Empty);

                        builder.Append(
                            $@"<li><a href=""{publishmentSystemUrl}"" target=""_blank""><i class=""icon-external-link""></i> 进入站点</a></li>");
                    }

                    builder.Append(
                            $@"<li><a href=""{HomeUtils.GetUrl(_publishmentSystemInfo.Additional.HomeUrl, string.Empty)}"" target=""_blank""><i class=""icon-external-link""></i> 进入用户中心</a></li>");

                    builder.Append(@"</ul>");

                    ltlMenues.Text = builder.ToString();
                }
                else
                {
                    e.Item.Visible = false;
                }
            }
            else if (index == 2)
            {

                ltlMenuLi.Text = @"<li>";

                var tabArrayList = ProductFileUtils.GetMenuTopArrayList();

                var builder = new StringBuilder();
                foreach (Tab tab in tabArrayList)
                {
                    if (TabManager.IsValid(tab, _permissions.PermissionList))
                    {
                        var loadingUrl = GetRedirectUrl(0, tab.Id);
                        if (!string.IsNullOrEmpty(tab.Href))
                        {
                            loadingUrl = PageUtils.ParseNavigationUrl(tab.Href);
                        }

                        var target = "_self";
                        if (!string.IsNullOrEmpty(tab.Target))
                        {
                            target = tab.Target;
                        }

                        //菜单平铺，update by sessionliang at 20151207
                        builder.Append(
                            $@"<span><a href=""{loadingUrl}"" target=""{target}"" style=""padding: 10px 15px;color: #fff;"">{tab
                                .Text}</a></span>");
                    }
                }

                if (builder.Length == 0)
                {
                    e.Item.Visible = false;
                }
                else
                {
                    ltlMenues.Text = builder.ToString();
                }
            }
        }

        private bool GetPublishmentSystemListHtml(StringBuilder builder)
        {

            var publishmentSystemIdList = ProductPermissionsManager.Current.PublishmentSystemIdList;

            //操作者拥有的站点列表
            var mySystemInfoArrayList = new ArrayList();

            var parentWithChildren = new Hashtable();

            if (ProductPermissionsManager.Current.IsSystemAdministrator)
            {
                foreach (var publishmentSystemId in publishmentSystemIdList)
                {
                    AddToMySystemInfoArrayList(mySystemInfoArrayList, parentWithChildren, publishmentSystemId);
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
                        AddToMySystemInfoArrayList(mySystemInfoArrayList, parentWithChildren, publishmentSystemId);
                    }
                }
            }

            if (_permissions.IsConsoleAdministrator)
            {
                var redirectUrl = PageAppAdd.GetRedirectUrl();
                builder.Append(
                    $@"<li style=""background:#eee;""><a href=""{PageUtils.GetLoadingUrl(redirectUrl)}""><i class=""icon-plus icon-large
""></i> 创建新站点</a></li>");

                builder.Append(@"<li class=""divider""></li>");
            }

            if (_hqPublishmentSystemInfo != null || mySystemInfoArrayList.Count > 0)
            {
                if (_hqPublishmentSystemInfo != null)
                {
                    AddSite(builder, _hqPublishmentSystemInfo, parentWithChildren, 0);
                }

                if (mySystemInfoArrayList.Count > 0)
                {
                    var count = 0;
                    foreach (PublishmentSystemInfo publishmentSystemInfo in mySystemInfoArrayList)
                    {
                        if (publishmentSystemInfo.IsHeadquarters == false)
                        {
                            count++;
                            AddSite(builder, publishmentSystemInfo, parentWithChildren, 0);
                        }
                        if (count == 13)
                        {
                            builder.Append(@"<li class=""divider""></li>");
                            builder.Append(
                                $@"<li style=""background:#eee;""><a href=""javascript:;"" onclick=""{ModalPublishmentSystemSelect
                                    .GetOpenLayerString()}""><i class=""icon-search icon-large
                    ""></i> 列出全部站点...</a></li>");
                            break;
                        }
                    }
                }
            }

            //if (builder.Length > 0)
            //{
            //    builder.Append(@"<li class=""divider""></li>");
            //}

            return _permissions.IsSystemAdministrator || publishmentSystemIdList.Count > 0;
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

        private void AddToMySystemInfoArrayList(ArrayList mySystemInfoArrayList, Hashtable parentWithChildren, int publishmentSystemId)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            if (publishmentSystemInfo != null)
            {
                if (publishmentSystemInfo.IsHeadquarters)
                {
                    _hqPublishmentSystemInfo = publishmentSystemInfo;
                }
                else if (publishmentSystemInfo.ParentPublishmentSystemId > 0)
                {
                    var children = new ArrayList();
                    if (parentWithChildren.Contains(publishmentSystemInfo.ParentPublishmentSystemId))
                    {
                        children = (ArrayList)parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId];
                    }
                    children.Add(publishmentSystemInfo);
                    parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId] = children;
                }
                mySystemInfoArrayList.Add(publishmentSystemInfo);
            }

        }

        private void AddSite(StringBuilder builder, PublishmentSystemInfo publishmentSystemInfo, Hashtable parentWithChildren, int level)
        {
            if (_addedSiteIdArrayList.Contains(publishmentSystemInfo.PublishmentSystemId)) return;

            var loadingUrl = PageUtils.GetLoadingUrl(GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, string.Empty));

            if (parentWithChildren[publishmentSystemInfo.PublishmentSystemId] != null)
            {
                var children = (ArrayList)parentWithChildren[publishmentSystemInfo.PublishmentSystemId];

                builder.Append($@"
<li class=""dropdown-submenu"">
    <a tabindex=""-1"" href=""{loadingUrl}"" target=""_self"">{EPublishmentSystemTypeUtils.GetIconHtml(
                    publishmentSystemInfo.PublishmentSystemType)}&nbsp;{publishmentSystemInfo.PublishmentSystemName}</a>
    <ul class=""dropdown-menu"">
");

                level++;
                foreach (PublishmentSystemInfo subSiteInfo in children)
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
                    $@"<li><a href=""{loadingUrl}"" target=""_self"">{EPublishmentSystemTypeUtils.GetIconHtml(
                        publishmentSystemInfo.PublishmentSystemType)}&nbsp;{publishmentSystemInfo.PublishmentSystemName}</a></li>");
            }

            _addedSiteIdArrayList.Add(publishmentSystemInfo.PublishmentSystemId);
        }
    }
}
