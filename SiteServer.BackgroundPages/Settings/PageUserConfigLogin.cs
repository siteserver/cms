using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageUserConfigLogin : BasePage
    {
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

            BreadCrumbSettings("用户登录配置", AppManager.Permissions.Settings.UserManagement);

            EBooleanUtils.AddListItems(RblIsFailToLock, "是", "否");
            ControlUtils.SelectListItemsIgnoreCase(RblIsFailToLock, ConfigManager.SystemConfigInfo.IsUserLockLogin.ToString());

            PhFailToLock.Visible = ConfigManager.SystemConfigInfo.IsUserLockLogin;

            TbLoginFailCount.Text = ConfigManager.SystemConfigInfo.UserLockLoginCount.ToString();

            DdlLockType.Items.Add(new ListItem("按小时锁定", EUserLockTypeUtils.GetValue(EUserLockType.Hours)));
            DdlLockType.Items.Add(new ListItem("永久锁定", EUserLockTypeUtils.GetValue(EUserLockType.Forever)));
            ControlUtils.SelectListItemsIgnoreCase(DdlLockType, ConfigManager.SystemConfigInfo.UserLockLoginType);
            TbLockingTime.Text = ConfigManager.SystemConfigInfo.UserLockLoginHours.ToString();

            PhLockingTime.Visible = false;
            PhLockingTime.Visible = EUserLockTypeUtils.Equals(ConfigManager.SystemConfigInfo.UserLockLoginType, EUserLockType.Hours);

            EBooleanUtils.AddListItems(RblIsFindPassword, "启用", "禁用");
            ControlUtils.SelectListItemsIgnoreCase(RblIsFindPassword, ConfigManager.SystemConfigInfo.IsUserFindPassword.ToString());
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
            if (Page.IsPostBack && Page.IsValid)
            {
                try
                {
                    ConfigManager.SystemConfigInfo.IsUserLockLogin = TranslateUtils.ToBool(RblIsFailToLock.SelectedValue);

                    ConfigManager.SystemConfigInfo.UserLockLoginCount = TranslateUtils.ToInt(TbLoginFailCount.Text, 3);

                    ConfigManager.SystemConfigInfo.UserLockLoginType = DdlLockType.SelectedValue;

                    ConfigManager.SystemConfigInfo.UserLockLoginHours = TranslateUtils.ToInt(TbLockingTime.Text);

                    ConfigManager.SystemConfigInfo.IsUserFindPassword = TranslateUtils.ToBool(RblIsFindPassword.SelectedValue);
                    ConfigManager.SystemConfigInfo.UserFindPasswordSmsTplId = TbFindPasswordSmsTplId.Text;

                    BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

                    Body.AddAdminLog("修改用户登录设置");

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
