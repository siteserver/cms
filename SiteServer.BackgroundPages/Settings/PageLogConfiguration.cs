using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageLogConfiguration : BasePage
    {
        protected RadioButtonList RblIsTimeThreshold;
        public PlaceHolder PhTimeThreshold;
        protected TextBox TbTime;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Log);

            EBooleanUtils.AddListItems(RblIsTimeThreshold, "启用", "不启用");
            ControlUtils.SelectSingleItem(RblIsTimeThreshold, ConfigManager.SystemConfigInfo.IsTimeThreshold.ToString());
            TbTime.Text = ConfigManager.SystemConfigInfo.TimeThreshold.ToString();

            PhTimeThreshold.Visible = TranslateUtils.ToBool(RblIsTimeThreshold.SelectedValue);
        }

        public void RblIsTimeThreshold_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhTimeThreshold.Visible = TranslateUtils.ToBool(RblIsTimeThreshold.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            ConfigManager.SystemConfigInfo.IsTimeThreshold = TranslateUtils.ToBool(RblIsTimeThreshold.SelectedValue);
            if (ConfigManager.SystemConfigInfo.IsTimeThreshold)
            {
                ConfigManager.SystemConfigInfo.TimeThreshold = TranslateUtils.ToInt(TbTime.Text);
            }

            DataProvider.ConfigDao.Update(ConfigManager.Instance);

            AuthRequest.AddAdminLog("设置日志阈值参数");
            SuccessMessage("日志设置成功");
        }
    }
}
