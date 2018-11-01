using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAdminConfiguration : BasePage
    {
        public TextBox TbLoginUserNameMinLength;
        public TextBox TbLoginPasswordMinLength;
        public DropDownList DdlLoginPasswordRestriction;

        public RadioButtonList RblIsLoginFailToLock;
        public PlaceHolder PhFailToLock;
        public TextBox TbLoginFailToLockCount;
        public DropDownList DdlLoginLockingType;
        public PlaceHolder PhLoginLockingHours;
        public TextBox TbLoginLockingHours;

        public RadioButtonList RblIsViewContentOnlySelf;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Admin);

            TbLoginUserNameMinLength.Text = ConfigManager.SystemConfigInfo.AdminUserNameMinLength.ToString();
            TbLoginPasswordMinLength.Text = ConfigManager.SystemConfigInfo.AdminPasswordMinLength.ToString();
            EUserPasswordRestrictionUtils.AddListItems(DdlLoginPasswordRestriction);
            ControlUtils.SelectSingleItemIgnoreCase(DdlLoginPasswordRestriction, ConfigManager.SystemConfigInfo.AdminPasswordRestriction);

            EBooleanUtils.AddListItems(RblIsLoginFailToLock, "是", "否");
            ControlUtils.SelectSingleItemIgnoreCase(RblIsLoginFailToLock, ConfigManager.SystemConfigInfo.IsAdminLockLogin.ToString());

            PhFailToLock.Visible = ConfigManager.SystemConfigInfo.IsAdminLockLogin;

            TbLoginFailToLockCount.Text = ConfigManager.SystemConfigInfo.AdminLockLoginCount.ToString();

            DdlLoginLockingType.Items.Add(new ListItem("按小时锁定", EUserLockTypeUtils.GetValue(EUserLockType.Hours)));
            DdlLoginLockingType.Items.Add(new ListItem("永久锁定", EUserLockTypeUtils.GetValue(EUserLockType.Forever)));
            ControlUtils.SelectSingleItemIgnoreCase(DdlLoginLockingType, ConfigManager.SystemConfigInfo.AdminLockLoginType);

            PhLoginLockingHours.Visible = false;
            if (!EUserLockTypeUtils.Equals(ConfigManager.SystemConfigInfo.AdminLockLoginType, EUserLockType.Forever))
            {
                PhLoginLockingHours.Visible = true;
                TbLoginLockingHours.Text = ConfigManager.SystemConfigInfo.AdminLockLoginHours.ToString();
            }

            EBooleanUtils.AddListItems(RblIsViewContentOnlySelf, "不可以", "可以");
            ControlUtils.SelectSingleItemIgnoreCase(RblIsViewContentOnlySelf, ConfigManager.SystemConfigInfo.IsViewContentOnlySelf.ToString());
        }

        public void RblIsLoginFailToLock_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhFailToLock.Visible = TranslateUtils.ToBool(RblIsLoginFailToLock.SelectedValue);
        }

        public void DdlLoginLockingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhLoginLockingHours.Visible = !EUserLockTypeUtils.Equals(EUserLockType.Forever, DdlLoginLockingType.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConfigManager.SystemConfigInfo.AdminUserNameMinLength = TranslateUtils.ToInt(TbLoginUserNameMinLength.Text);
                ConfigManager.SystemConfigInfo.AdminPasswordMinLength = TranslateUtils.ToInt(TbLoginPasswordMinLength.Text);
                ConfigManager.SystemConfigInfo.AdminPasswordRestriction = DdlLoginPasswordRestriction.SelectedValue;

                ConfigManager.SystemConfigInfo.IsAdminLockLogin = TranslateUtils.ToBool(RblIsLoginFailToLock.SelectedValue);
                ConfigManager.SystemConfigInfo.AdminLockLoginCount = TranslateUtils.ToInt(TbLoginFailToLockCount.Text, 3);
                ConfigManager.SystemConfigInfo.AdminLockLoginType = DdlLoginLockingType.SelectedValue;
                ConfigManager.SystemConfigInfo.AdminLockLoginHours = TranslateUtils.ToInt(TbLoginLockingHours.Text);

                ConfigManager.SystemConfigInfo.IsViewContentOnlySelf = TranslateUtils.ToBool(RblIsViewContentOnlySelf.SelectedValue);

                DataProvider.ConfigDao.Update(ConfigManager.Instance);

                AuthRequest.AddAdminLog("管理员设置");
                SuccessMessage("管理员设置成功");
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
    }
}
