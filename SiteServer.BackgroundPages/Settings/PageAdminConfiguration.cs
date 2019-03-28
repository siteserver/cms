using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
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

            TbLoginUserNameMinLength.Text = ConfigManager.Instance.AdminUserNameMinLength.ToString();
            TbLoginPasswordMinLength.Text = ConfigManager.Instance.AdminPasswordMinLength.ToString();
            EUserPasswordRestrictionUtils.AddListItems(DdlLoginPasswordRestriction);
            ControlUtils.SelectSingleItemIgnoreCase(DdlLoginPasswordRestriction, ConfigManager.Instance.AdminPasswordRestriction);

            EBooleanUtils.AddListItems(RblIsLoginFailToLock, "是", "否");
            ControlUtils.SelectSingleItemIgnoreCase(RblIsLoginFailToLock, ConfigManager.Instance.IsAdminLockLogin.ToString());

            PhFailToLock.Visible = ConfigManager.Instance.IsAdminLockLogin;

            TbLoginFailToLockCount.Text = ConfigManager.Instance.AdminLockLoginCount.ToString();

            DdlLoginLockingType.Items.Add(new ListItem("按小时锁定", EUserLockTypeUtils.GetValue(EUserLockType.Hours)));
            DdlLoginLockingType.Items.Add(new ListItem("永久锁定", EUserLockTypeUtils.GetValue(EUserLockType.Forever)));
            ControlUtils.SelectSingleItemIgnoreCase(DdlLoginLockingType, ConfigManager.Instance.AdminLockLoginType);

            PhLoginLockingHours.Visible = false;
            if (!EUserLockTypeUtils.Equals(ConfigManager.Instance.AdminLockLoginType, EUserLockType.Forever))
            {
                PhLoginLockingHours.Visible = true;
                TbLoginLockingHours.Text = ConfigManager.Instance.AdminLockLoginHours.ToString();
            }

            EBooleanUtils.AddListItems(RblIsViewContentOnlySelf, "不可以", "可以");
            ControlUtils.SelectSingleItemIgnoreCase(RblIsViewContentOnlySelf, ConfigManager.Instance.IsViewContentOnlySelf.ToString());
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
                ConfigManager.Instance.AdminUserNameMinLength = TranslateUtils.ToInt(TbLoginUserNameMinLength.Text);
                ConfigManager.Instance.AdminPasswordMinLength = TranslateUtils.ToInt(TbLoginPasswordMinLength.Text);
                ConfigManager.Instance.AdminPasswordRestriction = DdlLoginPasswordRestriction.SelectedValue;

                ConfigManager.Instance.IsAdminLockLogin = TranslateUtils.ToBool(RblIsLoginFailToLock.SelectedValue);
                ConfigManager.Instance.AdminLockLoginCount = TranslateUtils.ToInt(TbLoginFailToLockCount.Text, 3);
                ConfigManager.Instance.AdminLockLoginType = DdlLoginLockingType.SelectedValue;
                ConfigManager.Instance.AdminLockLoginHours = TranslateUtils.ToInt(TbLoginLockingHours.Text);

                ConfigManager.Instance.IsViewContentOnlySelf = TranslateUtils.ToBool(RblIsViewContentOnlySelf.SelectedValue);

                DataProvider.Config.Update(ConfigManager.Instance);

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
