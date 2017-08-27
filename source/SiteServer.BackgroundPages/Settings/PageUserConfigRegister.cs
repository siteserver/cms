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
                BreadCrumbSettings("用户注册配置", AppManager.Permissions.Settings.UserManagement);

                EBooleanUtils.AddListItems(RblIsRegisterAllowed);

                TbRegisterPasswordMinLength.Text = ConfigManager.UserConfigInfo.RegisterPasswordMinLength.ToString();

                EUserPasswordRestrictionUtils.AddListItems(DdlRegisterPasswordRestriction);
                ControlUtils.SelectListItemsIgnoreCase(DdlRegisterPasswordRestriction, EUserPasswordRestrictionUtils.GetValue(ConfigManager.UserConfigInfo.RegisterPasswordRestriction));

                EUserVerifyTypeUtils.AddListItems(DdlRegisterVerifyType);
                PhRegisterSms.Visible = ConfigManager.UserConfigInfo.RegisterVerifyType == EUserVerifyType.Mobile;
                TbRegisterSmsTplId.Text = ConfigManager.UserConfigInfo.RegisterSmsTplId;

                ControlUtils.SelectListItemsIgnoreCase(RblIsRegisterAllowed, ConfigManager.UserConfigInfo.IsRegisterAllowed.ToString());
                ControlUtils.SelectListItemsIgnoreCase(DdlRegisterVerifyType, EUserVerifyTypeUtils.GetValue(ConfigManager.UserConfigInfo.RegisterVerifyType));

                TbRegisterMinMinutesOfIpAddress.Text = ConfigManager.UserConfigInfo.RegisterMinMinutesOfIpAddress.ToString();
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
                ConfigManager.UserConfigInfo.IsRegisterAllowed = TranslateUtils.ToBool(RblIsRegisterAllowed.SelectedValue);

                ConfigManager.UserConfigInfo.RegisterPasswordMinLength = TranslateUtils.ToInt(TbRegisterPasswordMinLength.Text);
                ConfigManager.UserConfigInfo.RegisterPasswordRestriction = EUserPasswordRestrictionUtils.GetEnumType(DdlRegisterPasswordRestriction.SelectedValue);

                ConfigManager.UserConfigInfo.RegisterVerifyType = EUserVerifyTypeUtils.GetEnumType(DdlRegisterVerifyType.SelectedValue);
                ConfigManager.UserConfigInfo.RegisterSmsTplId = TbRegisterSmsTplId.Text;

                ConfigManager.UserConfigInfo.RegisterMinMinutesOfIpAddress = TranslateUtils.ToInt(TbRegisterMinMinutesOfIpAddress.Text);

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
