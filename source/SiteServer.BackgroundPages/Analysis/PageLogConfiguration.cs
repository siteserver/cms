using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Analysis
{
    public class PageLogConfiguration : BasePage
    {
        protected RadioButtonList rblIsTimeThreshold;
        public PlaceHolder phTimeThreshold;
        protected TextBox tbTime;
        public RadioButtonList rblIsCounterThreshold;
        public PlaceHolder phCounterThreshold;
        protected TextBox tbCounter;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbAnalysis(AppManager.Analysis.LeftMenu.Log, "日志阈值设置", AppManager.Analysis.Permission.AnalysisLog);

            EBooleanUtils.AddListItems(rblIsTimeThreshold, "启用", "不启用");
            ControlUtils.SelectListItemsIgnoreCase(rblIsTimeThreshold, ConfigManager.SystemConfigInfo.IsTimeThreshold.ToString());
            tbTime.Text = ConfigManager.SystemConfigInfo.TimeThreshold.ToString();

            EBooleanUtils.AddListItems(rblIsCounterThreshold, "启用", "不启用");
            ControlUtils.SelectListItemsIgnoreCase(rblIsCounterThreshold, ConfigManager.SystemConfigInfo.IsCounterThreshold.ToString());
            tbCounter.Text = ConfigManager.SystemConfigInfo.CounterThreshold.ToString();

            rblIsTimeThreshold_SelectedIndexChanged(null, EventArgs.Empty);
            rblIsCounterThreshold_SelectedIndexChanged(null, EventArgs.Empty);
        }

        public void rblIsTimeThreshold_SelectedIndexChanged(object sender, EventArgs e)
        {
            phTimeThreshold.Visible = TranslateUtils.ToBool(rblIsTimeThreshold.SelectedValue);
        }

        public void rblIsCounterThreshold_SelectedIndexChanged(object sender, EventArgs e)
        {
            phCounterThreshold.Visible = TranslateUtils.ToBool(rblIsCounterThreshold.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConfigManager.SystemConfigInfo.IsTimeThreshold = TranslateUtils.ToBool(rblIsTimeThreshold.SelectedValue);
                if (ConfigManager.SystemConfigInfo.IsTimeThreshold)
                {
                    ConfigManager.SystemConfigInfo.TimeThreshold = TranslateUtils.ToInt(tbTime.Text);
                }

                ConfigManager.SystemConfigInfo.IsCounterThreshold = TranslateUtils.ToBool(rblIsCounterThreshold.SelectedValue);
                if (ConfigManager.SystemConfigInfo.IsCounterThreshold)
                {
                    ConfigManager.SystemConfigInfo.CounterThreshold = TranslateUtils.ToInt(tbCounter.Text);
                }

                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

                Body.AddAdminLog("设置日志阈值参数");
                SuccessMessage("日志阈值参数设置成功");
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
    }
}
