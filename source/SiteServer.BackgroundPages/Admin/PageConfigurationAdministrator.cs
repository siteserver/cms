using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Admin
{
    public class PageConfigurationAdministrator : BasePage
    {
        public TextBox tbLoginUserNameMinLength;
        public TextBox tbLoginPasswordMinLength;
        public DropDownList ddlLoginPasswordRestriction;

        public RadioButtonList rblIsLoginFailToLock;
        public PlaceHolder phFailToLock;
        public TextBox tbLoginFailToLockCount;
        public DropDownList ddlLoginLockingType;
        public PlaceHolder phLoginLockingHours;
        public TextBox tbLoginLockingHours;

        public RadioButtonList rblIsViewContentOnlySelf;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumbAdmin(AppManager.Admin.LeftMenu.AdminConfiguration, "管理员设置", AppManager.Admin.Permission.AdminConfiguration);

                tbLoginUserNameMinLength.Text = ConfigManager.SystemConfigInfo.LoginUserNameMinLength.ToString();
                tbLoginPasswordMinLength.Text = ConfigManager.SystemConfigInfo.LoginPasswordMinLength.ToString();
                EUserPasswordRestrictionUtils.AddListItems(ddlLoginPasswordRestriction);
                ControlUtils.SelectListItemsIgnoreCase(ddlLoginPasswordRestriction, EUserPasswordRestrictionUtils.GetValue(ConfigManager.SystemConfigInfo.LoginPasswordRestriction));

                EBooleanUtils.AddListItems(rblIsLoginFailToLock, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(rblIsLoginFailToLock, ConfigManager.SystemConfigInfo.IsLoginFailToLock.ToString());

                phFailToLock.Visible = false;
                if (ConfigManager.SystemConfigInfo.IsLoginFailToLock)
                {
                    phFailToLock.Visible = true;
                }

                tbLoginFailToLockCount.Text = ConfigManager.SystemConfigInfo.LoginFailToLockCount.ToString();

                ddlLoginLockingType.Items.Add(new ListItem("按小时锁定", EUserLockTypeUtils.GetValue(EUserLockType.Hours)));
                ddlLoginLockingType.Items.Add(new ListItem("永久锁定", EUserLockTypeUtils.GetValue(EUserLockType.Forever)));
                ControlUtils.SelectListItemsIgnoreCase(ddlLoginLockingType, ConfigManager.SystemConfigInfo.LoginLockingType);

                phLoginLockingHours.Visible = false;
                if (!EUserLockTypeUtils.Equals(ConfigManager.SystemConfigInfo.LoginLockingType, EUserLockType.Forever))
                {
                    phLoginLockingHours.Visible = true;
                    tbLoginLockingHours.Text = ConfigManager.SystemConfigInfo.LoginLockingHours.ToString();
                }

                EBooleanUtils.AddListItems(rblIsViewContentOnlySelf, "不可以", "可以");
                ControlUtils.SelectListItemsIgnoreCase(rblIsViewContentOnlySelf, ConfigManager.SystemConfigInfo.IsViewContentOnlySelf.ToString());
            }
        }

        protected void rblIsLoginFailToLock_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TranslateUtils.ToBool(rblIsLoginFailToLock.SelectedValue))
            {
                phFailToLock.Visible = true;
            }
            else
            {
                phFailToLock.Visible = false;
            }
        }

        protected void ddlLoginLockingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!EUserLockTypeUtils.Equals(EUserLockType.Forever, ddlLoginLockingType.SelectedValue))
            {
                phLoginLockingHours.Visible = true;
            }
            else
            {
                phLoginLockingHours.Visible = false;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConfigManager.SystemConfigInfo.LoginUserNameMinLength = TranslateUtils.ToInt(tbLoginUserNameMinLength.Text);
                ConfigManager.SystemConfigInfo.LoginPasswordMinLength = TranslateUtils.ToInt(tbLoginPasswordMinLength.Text);
                ConfigManager.SystemConfigInfo.LoginPasswordRestriction = EUserPasswordRestrictionUtils.GetEnumType(ddlLoginPasswordRestriction.SelectedValue);

                ConfigManager.SystemConfigInfo.IsLoginFailToLock = TranslateUtils.ToBool(rblIsLoginFailToLock.SelectedValue);
                ConfigManager.SystemConfigInfo.LoginFailToLockCount = TranslateUtils.ToInt(tbLoginFailToLockCount.Text, 3);
                ConfigManager.SystemConfigInfo.LoginLockingType = ddlLoginLockingType.SelectedValue;
                ConfigManager.SystemConfigInfo.LoginLockingHours = TranslateUtils.ToInt(tbLoginLockingHours.Text);

                ConfigManager.SystemConfigInfo.IsViewContentOnlySelf = TranslateUtils.ToBool(rblIsViewContentOnlySelf.SelectedValue);

                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

                Body.AddAdminLog("管理员设置");
                SuccessMessage("管理员设置成功");
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
    }
}
