using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalPermissionsSet : BasePageCms
    {
        public DropDownList DdlPredefinedRole;
        public PlaceHolder PhSiteId;
        public CheckBoxList CblSiteId;
        public PlaceHolder PhRoles;
        public ListBox LbAvailableRoles;
        public ListBox LbAssignedRoles;

        private string _userName = string.Empty;

        public static string GetOpenWindowString(string userName)
        {
            return LayerUtils.GetOpenScript("权限设置",
                PageUtils.GetSettingsUrl(nameof(ModalPermissionsSet), new NameValueCollection
                {
                    {"UserName", userName}
                }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _userName = AuthRequest.GetQueryString("UserName");

            if (IsPostBack) return;

            var roles = DataProvider.AdministratorsInRolesDao.GetRolesForUser(_userName);
            if (AuthRequest.AdminPermissionsImpl.IsConsoleAdministrator)
            {
                DdlPredefinedRole.Items.Add(EPredefinedRoleUtils.GetListItem(EPredefinedRole.ConsoleAdministrator, false));
                DdlPredefinedRole.Items.Add(EPredefinedRoleUtils.GetListItem(EPredefinedRole.SystemAdministrator, false));
            }
            DdlPredefinedRole.Items.Add(EPredefinedRoleUtils.GetListItem(EPredefinedRole.Administrator, false));

            var type = EPredefinedRoleUtils.GetEnumTypeByRoles(roles);
            ControlUtils.SelectSingleItem(DdlPredefinedRole, EPredefinedRoleUtils.GetValue(type));

            var adminInfo = AdminManager.GetAdminInfoByUserName(_userName);
            var siteIdList = TranslateUtils.StringCollectionToIntList(adminInfo.SiteIdCollection);

            SiteManager.AddListItems(CblSiteId);
            ControlUtils.SelectMultiItems(CblSiteId, siteIdList);

            ListBoxDataBind();

            DdlPredefinedRole_SelectedIndexChanged(null, EventArgs.Empty);
        }

        public void DdlPredefinedRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EPredefinedRoleUtils.Equals(EPredefinedRole.ConsoleAdministrator, DdlPredefinedRole.SelectedValue))
            {
                PhRoles.Visible = PhSiteId.Visible = false;
            }
            else if (EPredefinedRoleUtils.Equals(EPredefinedRole.SystemAdministrator, DdlPredefinedRole.SelectedValue))
            {
                PhRoles.Visible = false;
                PhSiteId.Visible = true;
            }
            else
            {
                PhRoles.Visible = true;
                PhSiteId.Visible = false;
            }
        }

        private void ListBoxDataBind()
        {
            LbAvailableRoles.Items.Clear();
            LbAssignedRoles.Items.Clear();
            var allRoles = AuthRequest.AdminPermissionsImpl.IsConsoleAdministrator ? DataProvider.RoleDao.GetRoleNameList() : DataProvider.RoleDao.GetRoleNameListByCreatorUserName(AuthRequest.AdminName);
            var userRoles = DataProvider.AdministratorsInRolesDao.GetRolesForUser(_userName);
            var userRoleNameArrayList = new ArrayList(userRoles);
            foreach (var roleName in allRoles)
            {
                if (!EPredefinedRoleUtils.IsPredefinedRole(roleName) && !userRoleNameArrayList.Contains(roleName))
                {
                    LbAvailableRoles.Items.Add(new ListItem(roleName, roleName));
                }
            }
            foreach (var roleName in userRoles)
            {
                if (!EPredefinedRoleUtils.IsPredefinedRole(roleName))
                {
                    LbAssignedRoles.Items.Add(new ListItem(roleName, roleName));
                }
            }
        }

        public void AddRole_OnClick(object sender, EventArgs e)
        {
            if (!IsPostBack || !IsValid) return;

            try
            {
                if (LbAvailableRoles.SelectedIndex != -1)
                {
                    var selectedRoles = ControlUtils.GetSelectedListControlValueArray(LbAvailableRoles);
                    if (selectedRoles.Length > 0)
                    {
                        DataProvider.AdministratorsInRolesDao.AddUserToRoles(_userName, selectedRoles);
                    }
                }
                ListBoxDataBind();
            }
            catch (Exception ex)
            {
                FailMessage(ex, "用户角色分配失败");
            }
        }

        public void AddRoles_OnClick(object sender, EventArgs e)
        {
            if (!IsPostBack || !IsValid) return;

            try
            {
                var roles = ControlUtils.GetListControlValues(LbAvailableRoles);
                if (roles.Length > 0)
                {
                    DataProvider.AdministratorsInRolesDao.AddUserToRoles(_userName, roles);
                }
                ListBoxDataBind();
            }
            catch (Exception ex)
            {
                FailMessage(ex, "用户角色分配失败");
            }
        }

        public void DeleteRole_OnClick(object sender, EventArgs e)
        {
            if (!IsPostBack || !IsValid) return;

            try
            {
                if (LbAssignedRoles.SelectedIndex != -1)
                {
                    var selectedRoles = ControlUtils.GetSelectedListControlValueArray(LbAssignedRoles);
                    DataProvider.AdministratorsInRolesDao.RemoveUserFromRoles(_userName, selectedRoles);
                }
                ListBoxDataBind();
            }
            catch (Exception ex)
            {
                FailMessage(ex, "用户角色分配失败");
            }
        }

        public void DeleteRoles_OnClick(object sender, EventArgs e)
        {
            if (!IsPostBack || !IsValid) return;

            try
            {
                var roles = ControlUtils.GetListControlValues(LbAssignedRoles);
                if (roles.Length > 0)
                {
                    DataProvider.AdministratorsInRolesDao.RemoveUserFromRoles(_userName, roles);
                }
                ListBoxDataBind();
            }
            catch (Exception ex)
            {
                FailMessage(ex, "用户角色分配失败");
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                var allRoles = EPredefinedRoleUtils.GetAllPredefinedRoleName();
                foreach (var roleName in allRoles)
                {
                    DataProvider.AdministratorsInRolesDao.RemoveUserFromRole(_userName, roleName);
                }
                DataProvider.AdministratorsInRolesDao.AddUserToRole(_userName, DdlPredefinedRole.SelectedValue);

                var adminInfo = AdminManager.GetAdminInfoByUserName(_userName);

                DataProvider.AdministratorDao.UpdateSiteIdCollection(adminInfo,
                    EPredefinedRoleUtils.Equals(EPredefinedRole.SystemAdministrator, DdlPredefinedRole.SelectedValue)
                        ? ControlUtils.SelectedItemsValueToStringCollection(CblSiteId.Items)
                        : string.Empty);

                PermissionsImpl.ClearAllCache();

                AuthRequest.AddAdminLog("设置管理员权限", $"管理员:{_userName}");

                SuccessMessage("权限设置成功！");
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "权限设置失败！");
            }

            if (isChanged)
            {
                var redirectUrl = PageAdministrator.GetRedirectUrl();
                LayerUtils.CloseAndRedirect(Page, redirectUrl);
            }
        }
    }
}