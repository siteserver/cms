using System;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

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

            var config = ConfigManager.GetInstanceAsync().GetAwaiter().GetResult();

            EBooleanUtils.AddListItems(RblIsTimeThreshold, "启用", "不启用");
            ControlUtils.SelectSingleItem(RblIsTimeThreshold, config.IsTimeThreshold.ToString());
            TbTime.Text = config.TimeThreshold.ToString();

            PhTimeThreshold.Visible = TranslateUtils.ToBool(RblIsTimeThreshold.SelectedValue);
        }

        public void RblIsTimeThreshold_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhTimeThreshold.Visible = TranslateUtils.ToBool(RblIsTimeThreshold.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var config = ConfigManager.GetInstanceAsync().GetAwaiter().GetResult();

            config.IsTimeThreshold = TranslateUtils.ToBool(RblIsTimeThreshold.SelectedValue);
            if (config.IsTimeThreshold)
            {
                config.TimeThreshold = TranslateUtils.ToInt(TbTime.Text);
            }

            DataProvider.ConfigDao.UpdateAsync(config).GetAwaiter().GetResult();

            AuthRequest.AddAdminLogAsync("设置日志阈值参数").GetAwaiter().GetResult();
            SuccessMessage("日志设置成功");
        }
    }
}
