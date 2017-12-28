using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageUserConfigRegister : BasePage
    {
        public RadioButtonList RblIsRegisterAllowed;
        public TextBox TbRegisterPasswordMinLength;
        public DropDownList DdlRegisterPasswordRestriction;

        public DropDownList DdlRegisterVerifyType;
        public PlaceHolder PhRegisterSms;
        public TextBox TbRegisterSmsTplId;

        public TextBox TbRegisterMinMinutesOfIpAddress;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                VerifyAdministratorPermissions(AppManager.Permissions.Settings.UserManagement);

                EBooleanUtils.AddListItems(RblIsRegisterAllowed);

                TbRegisterPasswordMinLength.Text = ConfigManager.SystemConfigInfo.UserPasswordMinLength.ToString();

                EUserPasswordRestrictionUtils.AddListItems(DdlRegisterPasswordRestriction);
                ControlUtils.SelectSingleItemIgnoreCase(DdlRegisterPasswordRestriction, ConfigManager.SystemConfigInfo.UserPasswordRestriction);

                EUserVerifyTypeUtils.AddListItems(DdlRegisterVerifyType);
                PhRegisterSms.Visible = EUserVerifyTypeUtils.Equals(ConfigManager.SystemConfigInfo.UserRegistrationVerifyType, EUserVerifyType.Mobile);
                TbRegisterSmsTplId.Text = ConfigManager.SystemConfigInfo.UserRegistrationSmsTplId;

                ControlUtils.SelectSingleItemIgnoreCase(RblIsRegisterAllowed, ConfigManager.SystemConfigInfo.IsUserRegistrationAllowed.ToString());
                ControlUtils.SelectSingleItemIgnoreCase(DdlRegisterVerifyType, ConfigManager.SystemConfigInfo.UserRegistrationVerifyType);

                TbRegisterMinMinutesOfIpAddress.Text = ConfigManager.SystemConfigInfo.UserRegistrationMinMinutes.ToString();
            }
        }

        public void DdlRegisterVerifyType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhRegisterSms.Visible = EUserVerifyTypeUtils.Equals(DdlRegisterVerifyType.SelectedValue, EUserVerifyType.Mobile);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                ConfigManager.SystemConfigInfo.IsUserRegistrationAllowed = TranslateUtils.ToBool(RblIsRegisterAllowed.SelectedValue);

                ConfigManager.SystemConfigInfo.UserPasswordMinLength = TranslateUtils.ToInt(TbRegisterPasswordMinLength.Text);
                ConfigManager.SystemConfigInfo.UserPasswordRestriction = DdlRegisterPasswordRestriction.SelectedValue;

                ConfigManager.SystemConfigInfo.UserRegistrationVerifyType = DdlRegisterVerifyType.SelectedValue;
                ConfigManager.SystemConfigInfo.UserRegistrationSmsTplId = TbRegisterSmsTplId.Text;

                ConfigManager.SystemConfigInfo.UserRegistrationMinMinutes = TranslateUtils.ToInt(TbRegisterMinMinutesOfIpAddress.Text);

                try
                {
                    BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

                    Body.AddAdminLog("修改用户注册设置");

                    SuccessMessage("设置修改成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "设置修改失败！");
                }
            }
        }
    }
}
