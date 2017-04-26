using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.User
{
    public class PageUserConfigRegister : BasePage
    {
        public RadioButtonList RblIsRegisterAllowed;
        public TextBox TbRegisterPasswordMinLength;
        public DropDownList DdlRegisterPasswordRestriction;
        public DropDownList DdlRegisterVerifyType;
        public TextBox TbRegisterMinMinutesOfIpAddress;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumbUser(AppManager.User.LeftMenu.UserConfiguration, "用户注册配置", AppManager.User.Permission.UserConfiguration);

                EBooleanUtils.AddListItems(RblIsRegisterAllowed);

                TbRegisterPasswordMinLength.Text = ConfigManager.UserConfigInfo.RegisterPasswordMinLength.ToString();

                EUserPasswordRestrictionUtils.AddListItems(DdlRegisterPasswordRestriction);
                ControlUtils.SelectListItemsIgnoreCase(DdlRegisterPasswordRestriction, EUserPasswordRestrictionUtils.GetValue(ConfigManager.UserConfigInfo.RegisterPasswordRestriction));

                EUserVerifyTypeUtils.AddListItems(DdlRegisterVerifyType);

                ControlUtils.SelectListItemsIgnoreCase(RblIsRegisterAllowed, ConfigManager.UserConfigInfo.IsRegisterAllowed.ToString());
                ControlUtils.SelectListItemsIgnoreCase(DdlRegisterVerifyType, EUserVerifyTypeUtils.GetValue(ConfigManager.UserConfigInfo.RegisterVerifyType));

                TbRegisterMinMinutesOfIpAddress.Text = ConfigManager.UserConfigInfo.RegisterMinMinutesOfIpAddress.ToString();
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                ConfigManager.UserConfigInfo.IsRegisterAllowed = TranslateUtils.ToBool(RblIsRegisterAllowed.SelectedValue);

                ConfigManager.UserConfigInfo.RegisterPasswordMinLength = TranslateUtils.ToInt(TbRegisterPasswordMinLength.Text);
                ConfigManager.UserConfigInfo.RegisterPasswordRestriction = EUserPasswordRestrictionUtils.GetEnumType(DdlRegisterPasswordRestriction.SelectedValue);

                ConfigManager.UserConfigInfo.RegisterVerifyType = EUserVerifyTypeUtils.GetEnumType(DdlRegisterVerifyType.SelectedValue);

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
