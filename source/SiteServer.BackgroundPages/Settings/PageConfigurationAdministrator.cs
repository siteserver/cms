using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageConfigurationAdministrator : BasePage
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

        public RadioButtonList RblIsFindPassword;
        public PlaceHolder PhFindPassword;
        public TextBox TbFindPasswordSmsTplId;

        public RadioButtonList RblIsViewContentOnlySelf;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbSettings("管理员设置", AppManager.Permissions.Settings.AdminManagement);

            TbLoginUserNameMinLength.Text = ConfigManager.SystemConfigInfo.LoginUserNameMinLength.ToString();
            TbLoginPasswordMinLength.Text = ConfigManager.SystemConfigInfo.LoginPasswordMinLength.ToString();
            EUserPasswordRestrictionUtils.AddListItems(DdlLoginPasswordRestriction);
            ControlUtils.SelectListItemsIgnoreCase(DdlLoginPasswordRestriction, EUserPasswordRestrictionUtils.GetValue(ConfigManager.SystemConfigInfo.LoginPasswordRestriction));

            EBooleanUtils.AddListItems(RblIsLoginFailToLock, "是", "否");
            ControlUtils.SelectListItemsIgnoreCase(RblIsLoginFailToLock, ConfigManager.SystemConfigInfo.IsLoginFailToLock.ToString());

            PhFailToLock.Visible = ConfigManager.SystemConfigInfo.IsLoginFailToLock;

            TbLoginFailToLockCount.Text = ConfigManager.SystemConfigInfo.LoginFailToLockCount.ToString();

            DdlLoginLockingType.Items.Add(new ListItem("按小时锁定", EUserLockTypeUtils.GetValue(EUserLockType.Hours)));
            DdlLoginLockingType.Items.Add(new ListItem("永久锁定", EUserLockTypeUtils.GetValue(EUserLockType.Forever)));
            ControlUtils.SelectListItemsIgnoreCase(DdlLoginLockingType, ConfigManager.SystemConfigInfo.LoginLockingType);

            PhLoginLockingHours.Visible = false;
            if (!EUserLockTypeUtils.Equals(ConfigManager.SystemConfigInfo.LoginLockingType, EUserLockType.Forever))
            {
                PhLoginLockingHours.Visible = true;
                TbLoginLockingHours.Text = ConfigManager.SystemConfigInfo.LoginLockingHours.ToString();
            }

            EBooleanUtils.AddListItems(RblIsFindPassword, "启用", "禁用");
            ControlUtils.SelectListItemsIgnoreCase(RblIsFindPassword, ConfigManager.SystemConfigInfo.IsFindPassword.ToString());
            PhFindPassword.Visible = ConfigManager.SystemConfigInfo.IsFindPassword;
            TbFindPasswordSmsTplId.Text = ConfigManager.SystemConfigInfo.FindPasswordSmsTplId;

            EBooleanUtils.AddListItems(RblIsViewContentOnlySelf, "不可以", "可以");
            ControlUtils.SelectListItemsIgnoreCase(RblIsViewContentOnlySelf, ConfigManager.SystemConfigInfo.IsViewContentOnlySelf.ToString());
        }

        public void RblIsLoginFailToLock_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhFailToLock.Visible = TranslateUtils.ToBool(RblIsLoginFailToLock.SelectedValue);
        }

        public void DdlLoginLockingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhLoginLockingHours.Visible = !EUserLockTypeUtils.Equals(EUserLockType.Forever, DdlLoginLockingType.SelectedValue);
        }

        public void RblIsFindPassword_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhFindPassword.Visible = TranslateUtils.ToBool(RblIsFindPassword.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConfigManager.SystemConfigInfo.LoginUserNameMinLength = TranslateUtils.ToInt(TbLoginUserNameMinLength.Text);
                ConfigManager.SystemConfigInfo.LoginPasswordMinLength = TranslateUtils.ToInt(TbLoginPasswordMinLength.Text);
                ConfigManager.SystemConfigInfo.LoginPasswordRestriction = EUserPasswordRestrictionUtils.GetEnumType(DdlLoginPasswordRestriction.SelectedValue);

                ConfigManager.SystemConfigInfo.IsLoginFailToLock = TranslateUtils.ToBool(RblIsLoginFailToLock.SelectedValue);
                ConfigManager.SystemConfigInfo.LoginFailToLockCount = TranslateUtils.ToInt(TbLoginFailToLockCount.Text, 3);
                ConfigManager.SystemConfigInfo.LoginLockingType = DdlLoginLockingType.SelectedValue;
                ConfigManager.SystemConfigInfo.LoginLockingHours = TranslateUtils.ToInt(TbLoginLockingHours.Text);

                ConfigManager.SystemConfigInfo.IsFindPassword = TranslateUtils.ToBool(RblIsFindPassword.SelectedValue);
                ConfigManager.SystemConfigInfo.FindPasswordSmsTplId = TbFindPasswordSmsTplId.Text;

                ConfigManager.SystemConfigInfo.IsViewContentOnlySelf = TranslateUtils.ToBool(RblIsViewContentOnlySelf.SelectedValue);

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
