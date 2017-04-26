using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Configuration;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Admin
{
    public class PageRoleAdd : BasePageCms
    {
        public TextBox TbRoleName;
        public TextBox TbDescription;

        public Literal LtlPermissions;
        public PlaceHolder PhPublishmentSystemPermissions;
        public Literal LtlPublishmentSystems;

        public const string SystemPermissionsInfoListKey = "SystemPermissionsInfoListKey";

        private string _theRoleName;
        private List<string> _generalPermissionList;

        public static string GetReturnRedirectUrl(string roleName)
        {
            var queryString = new NameValueCollection {{"Return", "True"}};
            if (!string.IsNullOrEmpty(roleName))
            {
                queryString.Add("RoleName", roleName);
            }
            return PageUtils.GetAdminUrl(nameof(PageRoleAdd), queryString);
        }

        public string GetPublishmentSystemsHtml(ArrayList allPublishmentSystemIdArrayList, ArrayList managedPublishmentSystemIdArrayList)
        {
            var htmlBuilder = new StringBuilder();

            htmlBuilder.Append("<table width='100%' cellpadding='4' cellspacing='0' border='0'>");
            var count = 1;
            foreach (int publishmentSystemId in allPublishmentSystemIdArrayList)
            {
                var psInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var imageName = "cantedit";
                if (managedPublishmentSystemIdArrayList.Contains(publishmentSystemId))
                {
                    imageName = "canedit";
                }

                var space = "";
                if (count % 4 == 0)
                {
                    space = "<tr>";
                }

                var pageUrl = PageRoleAddPublishmentSystemPermissions.GetRedirectUrl(publishmentSystemId, Body.GetQueryString("RoleName"));
                string content = $@"
					<td height=20>
                        <img id='PublishmentSystemImage_{publishmentSystemId}' align='absmiddle' border='0' src='../pic/{imageName}.gif'/>
					    <a href='{pageUrl}'>{psInfo.PublishmentSystemName}&nbsp;{EPublishmentSystemTypeUtils.GetIconHtml(
                    psInfo.PublishmentSystemType)}</a>{space}
                    </td>
				";
                htmlBuilder.Append(content);
                count++;
            }
            htmlBuilder.Append("</table>");
            return htmlBuilder.ToString();
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var permissioins = PermissionsManager.GetPermissions(Body.AdministratorName);

            _theRoleName = Body.GetQueryString("RoleName");
            _generalPermissionList = permissioins.PermissionList;

            if (!IsPostBack)
            {
                AdminManager.VerifyAdministratorPermissions(Body.AdministratorName, AppManager.Admin.Permission.AdminManagement);

                if (!string.IsNullOrEmpty(_theRoleName))
                {
                    TbRoleName.Text = _theRoleName;
                    TbRoleName.Enabled = false;
                    TbDescription.Text = BaiRongDataProvider.RoleDao.GetRoleDescription(_theRoleName);

                    if (Body.GetQueryString("Return") == null)
                    {
                        var systemPermissionsInfoList = DataProvider.SystemPermissionsDao.GetSystemPermissionsInfoList(_theRoleName);
                        Session[SystemPermissionsInfoListKey] = systemPermissionsInfoList;
                    }
                }
                else
                {
                    if (Body.GetQueryString("Return") == null)
                    {
                        Session[SystemPermissionsInfoListKey] = new List<SystemPermissionsInfo>();
                    }
                }

                var cblPermissions = new CheckBoxList();

                var permissions = PermissionConfigManager.GetGeneralPermissionsOfProduct();
                if (permissions.Count > 0)
                {
                    foreach (PermissionConfig permission in permissions)
                    {
                        if (_generalPermissionList.Contains(permission.Name))
                        {
                            var listItem = new ListItem(permission.Text, permission.Name);
                            cblPermissions.Items.Add(listItem);
                        }
                    }

                    if (!string.IsNullOrEmpty(_theRoleName))
                    {
                        var permissionList = BaiRongDataProvider.PermissionsInRolesDao.GetGeneralPermissionList(new[] { _theRoleName });
                        if (permissionList != null && permissionList.Count > 0)
                        {
                            var permissionArray = new string[permissionList.Count];
                            permissionList.CopyTo(permissionArray);
                            ControlUtils.SelectListItems(cblPermissions, permissionArray);
                        }
                    }

                    var permissionBuilder = new StringBuilder();
                    permissionBuilder.Append(@"<table border=""0"" class=""table noborder""><tr>");
                    var i = 1;
                    var cblIndex = 0;
                    foreach (ListItem listItem in cblPermissions.Items)
                    {
                        permissionBuilder.Append(
                            $@"<td><label class=""checkbox""><input type=""checkbox"" id=""cblPermissions{cblIndex++}"" name=""Permissions"" {(listItem
                                .Selected
                                ? "checked"
                                : string.Empty)} value=""{listItem.Value}"" /> {listItem.Text}</label></td>");
                        if (i++ % 5 == 0)
                        {
                            permissionBuilder.Append("</tr><tr>");
                        }
                    }
                    permissionBuilder.Append(@"</tr></table>");
                    LtlPermissions.Text = permissionBuilder.ToString();
                }

                PhPublishmentSystemPermissions.Visible = false;

                var psPermissionsInRolesInfoList = Session[SystemPermissionsInfoListKey] as List<SystemPermissionsInfo>;
                if (psPermissionsInRolesInfoList != null)
                {
                    var allPublishmentSystemIdArrayList = new ArrayList();
                    foreach (var itemForPsid in ProductPermissionsManager.Current.WebsitePermissionDict.Keys)
                    {
                        if (ProductPermissionsManager.Current.ChannelPermissionDict.ContainsKey(itemForPsid) && ProductPermissionsManager.Current.WebsitePermissionDict.ContainsKey(itemForPsid))
                        {
                            var listOne = ProductPermissionsManager.Current.ChannelPermissionDict[itemForPsid];
                            var listTwo = ProductPermissionsManager.Current.WebsitePermissionDict[itemForPsid];
                            if ((listOne != null && listOne.Count > 0) || (listTwo != null && listTwo.Count > 0))
                            {
                                PhPublishmentSystemPermissions.Visible = true;
                                allPublishmentSystemIdArrayList.Add(itemForPsid);
                            }
                        }
                    }
                    var managedPublishmentSystemIdArrayList = new ArrayList();
                    foreach (var systemPermissionsInfo in psPermissionsInRolesInfoList)
                    {
                        managedPublishmentSystemIdArrayList.Add(systemPermissionsInfo.PublishmentSystemId);
                    }
                    LtlPublishmentSystems.Text = GetPublishmentSystemsHtml(allPublishmentSystemIdArrayList, managedPublishmentSystemIdArrayList);
                }
                else
                {
                    PageUtils.RedirectToErrorPage("页面超时，请重新进入。");
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (!string.IsNullOrEmpty(_theRoleName))
                {
                    try
                    {
                        var publishmentSystemPermissionsInRolesInfoList = Session[SystemPermissionsInfoListKey] as List<SystemPermissionsInfo>;
                        var generalPermissionArrayList = new ArrayList();

                        var productPermissions = Request["Permissions"];
                        if (!string.IsNullOrEmpty(productPermissions))
                        {
                            generalPermissionArrayList.AddRange(TranslateUtils.StringCollectionToStringList(productPermissions));
                        }

                        BaiRongDataProvider.PermissionsInRolesDao.UpdateRoleAndGeneralPermissions(_theRoleName, TbDescription.Text, generalPermissionArrayList);

                        DataProvider.PermissionsDao.UpdatePublishmentPermissions(_theRoleName, publishmentSystemPermissionsInRolesInfoList);

                        PermissionsManager.ClearAllCache();

                        Body.AddAdminLog("修改管理员角色", $"角色名称:{_theRoleName}");
                        SuccessMessage("角色修改成功！");
                        AddWaitAndRedirectScript(PageRole.GetRedirectUrl());
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "角色修改失败！");
                    }
                }
                else
                {
                    if (BaiRongDataProvider.RoleDao.IsRoleExists(TbRoleName.Text))
                    {
                        FailMessage("角色添加失败，角色标识已存在！");
                    }
                    else
                    {
                        var publishmentSystemPermissionsInRolesInfoList = Session[SystemPermissionsInfoListKey] as List<SystemPermissionsInfo>;
                        var generalPermissionList = new List<string>();
                        
                        var permissions = Request["Permissions"];
                        if (!string.IsNullOrEmpty(permissions))
                        {
                            generalPermissionList.AddRange(TranslateUtils.StringCollectionToStringList(permissions));
                        }

                        try
                        {
                            DataProvider.PermissionsDao.InsertRoleAndPermissions(TbRoleName.Text, Body.AdministratorName, TbDescription.Text, generalPermissionList, publishmentSystemPermissionsInRolesInfoList);

                            PermissionsManager.ClearAllCache();

                            Body.AddAdminLog("新增管理员角色",
                                $"角色名称:{TbRoleName.Text}");

                            SuccessMessage("角色添加成功！");
                            AddWaitAndRedirectScript(PageRole.GetRedirectUrl());
                        }
                        catch (Exception ex)
                        {
                            FailMessage(ex, $"角色添加失败，{ex.Message}");
                        }
                    }
                }

            }
        }

    }
}
