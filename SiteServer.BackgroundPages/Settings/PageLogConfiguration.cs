using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
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
            ControlUtils.SelectSingleItem(RblIsTimeThreshold, ConfigManager.Instance.IsTimeThreshold.ToString());
            TbTime.Text = ConfigManager.Instance.TimeThreshold.ToString();

            PhTimeThreshold.Visible = TranslateUtils.ToBool(RblIsTimeThreshold.SelectedValue);
        }

        public void RblIsTimeThreshold_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhTimeThreshold.Visible = TranslateUtils.ToBool(RblIsTimeThreshold.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            ConfigManager.Instance.IsTimeThreshold = TranslateUtils.ToBool(RblIsTimeThreshold.SelectedValue);
            if (ConfigManager.Instance.IsTimeThreshold)
            {
                ConfigManager.Instance.TimeThreshold = TranslateUtils.ToInt(TbTime.Text);
            }

            DataProvider.Config.Update(ConfigManager.Instance);

            AuthRequest.AddAdminLog("设置日志阈值参数");
            SuccessMessage("日志设置成功");
        }
    }
}
