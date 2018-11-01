using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAdminRoleAdd : BasePageCms
    {
        public TextBox TbRoleName;
        public TextBox TbDescription;
        public CheckBoxList CblPermissions;
        public PlaceHolder PhSitePermissions;
        public Literal LtlSites;
        public Button BtnReturn;

        public const string SystemPermissionsInfoListKey = "SystemPermissionsInfoListKey";
        public string StrCookie { get; set; }

        private string _theRoleName;
        private List<string> _generalPermissionList;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageAdminRoleAdd), null);
        }

        public static string GetRedirectUrl(string roleName)
        {
            return PageUtils.GetSettingsUrl(nameof(PageAdminRoleAdd), new NameValueCollection { { "RoleName", roleName } });
        }

        public static string GetReturnRedirectUrl(string roleName)
        {
            var queryString = new NameValueCollection {{"Return", "True"}};
            if (!string.IsNullOrEmpty(roleName))
            {
                queryString.Add("RoleName", roleName);
            }
            return PageUtils.GetSettingsUrl(nameof(PageAdminRoleAdd), queryString);
        }

        public string GetSitesHtml(List<int> allSiteIdList, List<int> managedSiteIdList)
        {
            var htmlBuilder = new StringBuilder();

            foreach (var siteId in allSiteIdList)
            {
                var psInfo = SiteManager.GetSiteInfo(siteId);
                var className = "bg-light";
                if (managedSiteIdList.Contains(siteId))
                {
                    className = "bg-primary text-white";
                }

                var pageUrl = PageAdminPermissionAdd.GetRedirectUrl(siteId, AuthRequest.GetQueryString("RoleName"));
                string content = $@"
<div onclick=""location.href = '{pageUrl}'"" class=""card {className} mb-3 ml-3 float-left"" style=""max-width: 18rem;cursor: pointer;"">
    <div class=""card-header"">{psInfo.SiteName}</div>
    <div class=""card-body"">
        <p class=""card-text"">文件夹：{psInfo.SiteDir}</p>
        <p class=""card-text"">创建日期：{DateUtils.GetDateString(ChannelManager.GetAddDate(siteId, siteId))}</p>
    </div>
</div>";
                htmlBuilder.Append(content);
            }

            return htmlBuilder.ToString();
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _theRoleName = AuthRequest.GetQueryString("RoleName");
            _generalPermissionList = AuthRequest.AdminPermissionsImpl.PermissionList;

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Admin);

            if (!string.IsNullOrEmpty(_theRoleName))
            {
                TbRoleName.Text = _theRoleName;
                TbRoleName.Enabled = false;
                TbDescription.Text = DataProvider.RoleDao.GetRoleDescription(_theRoleName);

                if (AuthRequest.GetQueryString("Return") == null)
                {
                    var systemPermissionsInfoList = DataProvider.SitePermissionsDao.GetSystemPermissionsInfoList(_theRoleName);
                    Session[SystemPermissionsInfoListKey] = systemPermissionsInfoList;
                }
            }
            else
            {
                if (AuthRequest.GetQueryString("Return") == null)
                {
                    Session[SystemPermissionsInfoListKey] = new List<SitePermissionsInfo>();
                }
            }

            var permissions = PermissionConfigManager.Instance.GeneralPermissions;
            if (permissions.Count > 0)
            {
                foreach (var permission in permissions)
                {
                    if (_generalPermissionList.Contains(permission.Name))
                    {
                        var listItem = new ListItem(permission.Text, permission.Name);
                        CblPermissions.Items.Add(listItem);
                    }
                }

                if (!string.IsNullOrEmpty(_theRoleName))
                {
                    var permissionList = DataProvider.PermissionsInRolesDao.GetGeneralPermissionList(new[] { _theRoleName });
                    if (permissionList != null && permissionList.Count > 0)
                    {
                        ControlUtils.SelectMultiItems(CblPermissions, permissionList);
                    }
                }
            }

            PhSitePermissions.Visible = false;

            var psPermissionsInRolesInfoList = Session[SystemPermissionsInfoListKey] as List<SitePermissionsInfo>;
            if (psPermissionsInRolesInfoList != null)
            {
                var allSiteIdList = new List<int>();
                foreach (var permissionSiteId in AuthRequest.AdminPermissionsImpl.GetSiteIdList())
                {
                    if (AuthRequest.AdminPermissionsImpl.HasChannelPermissions(permissionSiteId, permissionSiteId) && AuthRequest.AdminPermissionsImpl.HasSitePermissions(permissionSiteId))
                    {
                        var listOne = AuthRequest.AdminPermissionsImpl.GetChannelPermissions(permissionSiteId, permissionSiteId);
                        var listTwo = AuthRequest.AdminPermissionsImpl.GetSitePermissions(permissionSiteId);
                        if (listOne != null && listOne.Count > 0 || listTwo != null && listTwo.Count > 0)
                        {
                            PhSitePermissions.Visible = true;
                            allSiteIdList.Add(permissionSiteId);
                        }
                    }
                }
                var managedSiteIdList = new List<int>();
                foreach (var systemPermissionsInfo in psPermissionsInRolesInfoList)
                {
                    managedSiteIdList.Add(systemPermissionsInfo.SiteId);
                }
                LtlSites.Text = GetSitesHtml(allSiteIdList, managedSiteIdList);
            }
            else
            {
                PageUtils.RedirectToErrorPage("页面超时，请重新进入。");
            }

            if (Request.QueryString["Return"] == null)
            {
                BtnReturn.Visible = false;
                StrCookie = @"_setCookie(""pageRoleAdd"", """");";
            }
            else
            {
                StrCookie = @"
var ss_role = _getCookie(""pageRoleAdd"");
if (ss_role) {
    var strs = ss_role.split("","");
    for (i = 0; i < strs.length; i++) {
        var el = document.getElementById(strs[i].split("":"")[0]);
        if (el.type == ""checkbox"") {
            el.checked = (strs[i].split("":"")[1] == ""true"");
        } else {
            el.value = strs[i].split("":"")[1];
        }
    }
}
";
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            if (!string.IsNullOrEmpty(_theRoleName))
            {
                try
                {
                    var sitePermissionsInRolesInfoList = Session[SystemPermissionsInfoListKey] as List<SitePermissionsInfo>;

                    var generalPermissionList = ControlUtils.GetSelectedListControlValueStringList(CblPermissions);
                    DataProvider.PermissionsInRolesDao.UpdateRoleAndGeneralPermissions(_theRoleName, TbDescription.Text, generalPermissionList);

                    DataProvider.SitePermissionsDao.UpdateSitePermissions(_theRoleName, sitePermissionsInRolesInfoList);

                    PermissionsImpl.ClearAllCache();

                    AuthRequest.AddAdminLog("修改管理员角色", $"角色名称:{_theRoleName}");
                    SuccessMessage("角色修改成功！");
                    AddWaitAndRedirectScript(PageAdminRole.GetRedirectUrl());
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "角色修改失败！");
                }
            }
            else
            {
                if (EPredefinedRoleUtils.IsPredefinedRole(TbRoleName.Text))
                {
                    FailMessage($"角色添加失败，{TbRoleName.Text}为系统角色！");
                }
                else if (DataProvider.RoleDao.IsRoleExists(TbRoleName.Text))
                {
                    FailMessage("角色添加失败，角色标识已存在！");
                }
                else
                {
                    var sitePermissionsInRolesInfoList = Session[SystemPermissionsInfoListKey] as List<SitePermissionsInfo>;
                    var generalPermissionList = ControlUtils.GetSelectedListControlValueStringList(CblPermissions);

                    try
                    {
                        DataProvider.SitePermissionsDao.InsertRoleAndPermissions(TbRoleName.Text, AuthRequest.AdminName, TbDescription.Text, generalPermissionList, sitePermissionsInRolesInfoList);

                        PermissionsImpl.ClearAllCache();

                        AuthRequest.AddAdminLog("新增管理员角色",
                            $"角色名称:{TbRoleName.Text}");

                        SuccessMessage("角色添加成功！");
                        AddWaitAndRedirectScript(PageAdminRole.GetRedirectUrl());
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, $"角色添加失败，{ex.Message}");
                    }
                }
            }
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            if (AuthRequest.GetQueryString("Return") != null)
            {
                PageUtils.Redirect(PageAdminRole.GetRedirectUrl());
            }
        }
    }
}
