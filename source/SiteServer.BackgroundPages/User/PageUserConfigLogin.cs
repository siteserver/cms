using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.User
{
    public class PageUserConfigLogin : BasePage
    {
        public RadioButtonList rblIsRecordIP;
        public RadioButtonList rblIsRecordSource;
        public RadioButtonList rblIsFailToLock;
        public DropDownList ddlLockType;
        public TextBox loginFailCount;
        public TextBox lockingTime;
        public PlaceHolder phLockingTime;
        public PlaceHolder phFailToLock;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumbUser(AppManager.User.LeftMenu.UserConfiguration, "用户登录配置", AppManager.User.Permission.UserConfiguration);

                EBooleanUtils.AddListItems(rblIsRecordIP, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(rblIsRecordIP, ConfigManager.UserConfigInfo.IsRecordIp.ToString());
                EBooleanUtils.AddListItems(rblIsRecordSource, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(rblIsRecordSource, ConfigManager.UserConfigInfo.IsRecordSource.ToString());
                EBooleanUtils.AddListItems(rblIsFailToLock, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(rblIsFailToLock, ConfigManager.UserConfigInfo.IsLoginFailToLock.ToString());

                phFailToLock.Visible = false;
                if (ConfigManager.UserConfigInfo.IsLoginFailToLock)
                {
                    phFailToLock.Visible = true;
                }

                loginFailCount.Text = ConfigManager.UserConfigInfo.LoginFailToLockCount.ToString();

                ddlLockType.Items.Add(new ListItem("按小时锁定", EUserLockTypeUtils.GetValue(EUserLockType.Hours)));
                ddlLockType.Items.Add(new ListItem("永久锁定", EUserLockTypeUtils.GetValue(EUserLockType.Forever)));
                ControlUtils.SelectListItemsIgnoreCase(ddlLockType, ConfigManager.UserConfigInfo.LoginLockingType);
                lockingTime.Text = ConfigManager.UserConfigInfo.LoginLockingHours.ToString();

                phLockingTime.Visible = false;
                phLockingTime.Visible = EUserLockTypeUtils.Equals(ConfigManager.UserConfigInfo.LoginLockingType, EUserLockType.Hours);
            }
        }

        protected void rblIsFailToLock_SelectedIndexChanged(object sender, EventArgs e)
        {
            phFailToLock.Visible = TranslateUtils.ToBool(rblIsFailToLock.SelectedValue);
        }

        protected void ddlLockType_SelectedIndexChanged(object sender, EventArgs e)
        {
            phLockingTime.Visible = !EUserLockTypeUtils.Equals(EUserLockType.Forever, ddlLockType.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                try
                {
                    ConfigManager.UserConfigInfo.IsRecordIp = TranslateUtils.ToBool(rblIsRecordIP.SelectedValue);
                    ConfigManager.UserConfigInfo.IsRecordSource = TranslateUtils.ToBool(rblIsRecordSource.SelectedValue);
                    ConfigManager.UserConfigInfo.IsLoginFailToLock = TranslateUtils.ToBool(rblIsFailToLock.SelectedValue);

                    ConfigManager.UserConfigInfo.LoginFailToLockCount = TranslateUtils.ToInt(loginFailCount.Text, 3);

                    ConfigManager.UserConfigInfo.LoginLockingType = ddlLockType.SelectedValue;

                    ConfigManager.UserConfigInfo.LoginLockingHours = TranslateUtils.ToInt(lockingTime.Text);

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
