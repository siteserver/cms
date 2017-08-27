using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageUserConfigLogin : BasePage
    {
        public RadioButtonList RblIsRecordIp;
        public RadioButtonList RblIsRecordSource;
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

            EBooleanUtils.AddListItems(RblIsRecordIp, "是", "否");
            ControlUtils.SelectListItemsIgnoreCase(RblIsRecordIp, ConfigManager.UserConfigInfo.IsRecordIp.ToString());
            EBooleanUtils.AddListItems(RblIsRecordSource, "是", "否");
            ControlUtils.SelectListItemsIgnoreCase(RblIsRecordSource, ConfigManager.UserConfigInfo.IsRecordSource.ToString());
            EBooleanUtils.AddListItems(RblIsFailToLock, "是", "否");
            ControlUtils.SelectListItemsIgnoreCase(RblIsFailToLock, ConfigManager.UserConfigInfo.IsLoginFailToLock.ToString());

            PhFailToLock.Visible = ConfigManager.UserConfigInfo.IsLoginFailToLock;

            TbLoginFailCount.Text = ConfigManager.UserConfigInfo.LoginFailToLockCount.ToString();

            DdlLockType.Items.Add(new ListItem("按小时锁定", EUserLockTypeUtils.GetValue(EUserLockType.Hours)));
            DdlLockType.Items.Add(new ListItem("永久锁定", EUserLockTypeUtils.GetValue(EUserLockType.Forever)));
            ControlUtils.SelectListItemsIgnoreCase(DdlLockType, ConfigManager.UserConfigInfo.LoginLockingType);
            TbLockingTime.Text = ConfigManager.UserConfigInfo.LoginLockingHours.ToString();

            PhLockingTime.Visible = false;
            PhLockingTime.Visible = EUserLockTypeUtils.Equals(ConfigManager.UserConfigInfo.LoginLockingType, EUserLockType.Hours);

            EBooleanUtils.AddListItems(RblIsFindPassword, "启用", "禁用");
            ControlUtils.SelectListItemsIgnoreCase(RblIsFindPassword, ConfigManager.UserConfigInfo.IsFindPassword.ToString());
            PhFindPassword.Visible = ConfigManager.UserConfigInfo.IsFindPassword;
            TbFindPasswordSmsTplId.Text = ConfigManager.UserConfigInfo.FindPasswordSmsTplId;
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
                    ConfigManager.UserConfigInfo.IsRecordIp = TranslateUtils.ToBool(RblIsRecordIp.SelectedValue);
                    ConfigManager.UserConfigInfo.IsRecordSource = TranslateUtils.ToBool(RblIsRecordSource.SelectedValue);
                    ConfigManager.UserConfigInfo.IsLoginFailToLock = TranslateUtils.ToBool(RblIsFailToLock.SelectedValue);

                    ConfigManager.UserConfigInfo.LoginFailToLockCount = TranslateUtils.ToInt(TbLoginFailCount.Text, 3);

                    ConfigManager.UserConfigInfo.LoginLockingType = DdlLockType.SelectedValue;

                    ConfigManager.UserConfigInfo.LoginLockingHours = TranslateUtils.ToInt(TbLockingTime.Text);

                    ConfigManager.UserConfigInfo.IsFindPassword = TranslateUtils.ToBool(RblIsFindPassword.SelectedValue);
                    ConfigManager.UserConfigInfo.FindPasswordSmsTplId = TbFindPasswordSmsTplId.Text;

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
