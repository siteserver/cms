using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageUserConfiguration : BasePage
    {
        public RadioButtonList RblIsRegisterAllowed;
        public TextBox TbRegisterPasswordMinLength;
        public DropDownList DdlRegisterPasswordRestriction;
        public TextBox TbRegisterMinMinutesOfIpAddress;

        public RadioButtonList RblIsFailToLock;
        public PlaceHolder PhFailToLock;
        public DropDownList DdlLockType;
        public TextBox TbLoginFailCount;
        public PlaceHolder PhLockingTime;
        public TextBox TbLockingTime;
        public RadioButtonList RblIsFindPassword;
        public PlaceHolder PhFindPassword;
        public TextBox TbFindPasswordSmsTplId;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.User);

            EBooleanUtils.AddListItems(RblIsRegisterAllowed);

            TbRegisterPasswordMinLength.Text = ConfigManager.SystemConfigInfo.UserPasswordMinLength.ToString();

            EUserPasswordRestrictionUtils.AddListItems(DdlRegisterPasswordRestriction);
            ControlUtils.SelectSingleItemIgnoreCase(DdlRegisterPasswordRestriction, ConfigManager.SystemConfigInfo.UserPasswordRestriction);

            ControlUtils.SelectSingleItemIgnoreCase(RblIsRegisterAllowed, ConfigManager.SystemConfigInfo.IsUserRegistrationAllowed.ToString());

            TbRegisterMinMinutesOfIpAddress.Text = ConfigManager.SystemConfigInfo.UserRegistrationMinMinutes.ToString();

            EBooleanUtils.AddListItems(RblIsFailToLock, "是", "否");
            ControlUtils.SelectSingleItemIgnoreCase(RblIsFailToLock, ConfigManager.SystemConfigInfo.IsUserLockLogin.ToString());

            PhFailToLock.Visible = ConfigManager.SystemConfigInfo.IsUserLockLogin;

            TbLoginFailCount.Text = ConfigManager.SystemConfigInfo.UserLockLoginCount.ToString();

            DdlLockType.Items.Add(new ListItem("按小时锁定", EUserLockTypeUtils.GetValue(EUserLockType.Hours)));
            DdlLockType.Items.Add(new ListItem("永久锁定", EUserLockTypeUtils.GetValue(EUserLockType.Forever)));
            ControlUtils.SelectSingleItemIgnoreCase(DdlLockType, ConfigManager.SystemConfigInfo.UserLockLoginType);
            TbLockingTime.Text = ConfigManager.SystemConfigInfo.UserLockLoginHours.ToString();

            PhLockingTime.Visible = false;
            PhLockingTime.Visible = EUserLockTypeUtils.Equals(ConfigManager.SystemConfigInfo.UserLockLoginType, EUserLockType.Hours);

            EBooleanUtils.AddListItems(RblIsFindPassword, "启用", "禁用");
            ControlUtils.SelectSingleItemIgnoreCase(RblIsFindPassword, ConfigManager.SystemConfigInfo.IsUserFindPassword.ToString());
            PhFindPassword.Visible = ConfigManager.SystemConfigInfo.IsUserFindPassword;
            TbFindPasswordSmsTplId.Text = ConfigManager.SystemConfigInfo.UserFindPasswordSmsTplId;
        }

        public void RblIsFailToLock_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhFailToLock.Visible = TranslateUtils.ToBool(RblIsFailToLock.SelectedValue);
        }

        public void DdlLockType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhLockingTime.Visible = !EUserLockTypeUtils.Equals(EUserLockType.Forever, DdlLockType.SelectedValue);
        }

        public void RblIsFindPassword_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhFindPassword.Visible = TranslateUtils.ToBool(RblIsFindPassword.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            ConfigManager.SystemConfigInfo.IsUserRegistrationAllowed = TranslateUtils.ToBool(RblIsRegisterAllowed.SelectedValue);
            ConfigManager.SystemConfigInfo.UserPasswordMinLength = TranslateUtils.ToInt(TbRegisterPasswordMinLength.Text);
            ConfigManager.SystemConfigInfo.UserPasswordRestriction = DdlRegisterPasswordRestriction.SelectedValue;
            ConfigManager.SystemConfigInfo.UserRegistrationMinMinutes = TranslateUtils.ToInt(TbRegisterMinMinutesOfIpAddress.Text);

            ConfigManager.SystemConfigInfo.IsUserLockLogin = TranslateUtils.ToBool(RblIsFailToLock.SelectedValue);
            ConfigManager.SystemConfigInfo.UserLockLoginCount = TranslateUtils.ToInt(TbLoginFailCount.Text, 3);
            ConfigManager.SystemConfigInfo.UserLockLoginType = DdlLockType.SelectedValue;
            ConfigManager.SystemConfigInfo.UserLockLoginHours = TranslateUtils.ToInt(TbLockingTime.Text);
            ConfigManager.SystemConfigInfo.IsUserFindPassword = TranslateUtils.ToBool(RblIsFindPassword.SelectedValue);
            ConfigManager.SystemConfigInfo.UserFindPasswordSmsTplId = TbFindPasswordSmsTplId.Text;

            DataProvider.ConfigDao.Update(ConfigManager.Instance);

            AuthRequest.AddAdminLog("修改用户设置");

            SuccessMessage("设置修改成功！");
        }
    }
}
