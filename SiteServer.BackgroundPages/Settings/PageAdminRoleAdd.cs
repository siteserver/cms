using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;

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
                var className = "bg-secondary";
                if (managedSiteIdList.Contains(siteId))
                {
                    className = "bg-primary";
                }

                var pageUrl = PageAdminPermissionAdd.GetRedirectUrl(siteId, Body.GetQueryString("RoleName"));
                string content = $@"
<div onclick=""location.href = '{pageUrl}'"" class=""card text-white {className} mb-3 ml-3 float-left"" style=""max-width: 18rem;cursor: pointer;"">
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

            var permissioins = PermissionsManager.GetPermissions(Body.AdminName);

            _theRoleName = Body.GetQueryString("RoleName");
            _generalPermissionList = permissioins.PermissionList;

            if (IsPostBack) return;

            PermissionsManager.VerifyAdministratorPermissions(Body.AdminName, ConfigManager.Permissions.Settings.Admin);

            if (!string.IsNullOrEmpty(_theRoleName))
            {
                TbRoleName.Text = _theRoleName;
                TbRoleName.Enabled = false;
                TbDescription.Text = DataProvider.RoleDao.GetRoleDescription(_theRoleName);

                if (Body.GetQueryString("Return") == null)
                {
                    var systemPermissionsInfoList = DataProvider.SitePermissionsDao.GetSystemPermissionsInfoList(_theRoleName);
                    Session[SystemPermissionsInfoListKey] = systemPermissionsInfoList;
                }
            }
            else
            {
                if (Body.GetQueryString("Return") == null)
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
                foreach (var itemForPsid in ProductPermissionsManager.Current.WebsitePermissionDict.Keys)
                {
                    if (ProductPermissionsManager.Current.ChannelPermissionDict.ContainsKey(itemForPsid) && ProductPermissionsManager.Current.WebsitePermissionDict.ContainsKey(itemForPsid))
                    {
                        var listOne = ProductPermissionsManager.Current.ChannelPermissionDict[itemForPsid];
                        var listTwo = ProductPermissionsManager.Current.WebsitePermissionDict[itemForPsid];
                        if (listOne != null && listOne.Count > 0 || listTwo != null && listTwo.Count > 0)
                        {
                            PhSitePermissions.Visible = true;
                            allSiteIdList.Add(itemForPsid);
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

                    PermissionsManager.ClearAllCache();

                    Body.AddAdminLog("修改管理员角色", $"角色名称:{_theRoleName}");
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
                if (DataProvider.RoleDao.IsRoleExists(TbRoleName.Text))
                {
                    FailMessage("角色添加失败，角色标识已存在！");
                }
                else
                {
                    var sitePermissionsInRolesInfoList = Session[SystemPermissionsInfoListKey] as List<SitePermissionsInfo>;
                    var generalPermissionList = ControlUtils.GetSelectedListControlValueStringList(CblPermissions);

                    try
                    {
                        DataProvider.SitePermissionsDao.InsertRoleAndPermissions(TbRoleName.Text, Body.AdminName, TbDescription.Text, generalPermissionList, sitePermissionsInRolesInfoList);

                        PermissionsManager.ClearAllCache();

                        Body.AddAdminLog("新增管理员角色",
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
            if (Body.GetQueryString("Return") != null)
            {
                PageUtils.Redirect(PageAdminRole.GetRedirectUrl());
            }
        }
    }
}
