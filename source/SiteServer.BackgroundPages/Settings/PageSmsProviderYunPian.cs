using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSmsProviderYunPian : BasePage
    {
        public Literal LtlType;

        public TextBox TbApiKey;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageSmsProviderYunPian), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumbSettings(AppManager.Settings.LeftMenu.Config, "短信服务商管理", AppManager.Settings.Permission.SettingsConfig);

                LtlType.Text = $@"{ESmsProviderTypeUtils.GetText(ESmsProviderType.YunPian)}(<a href=""{ESmsProviderTypeUtils.GetUrl(ESmsProviderType.YunPian)}"" target=""_blank"">{ESmsProviderTypeUtils.GetUrl(ESmsProviderType.YunPian)}</a>)";

                TbApiKey.Text = ConfigManager.SystemConfigInfo.SmsYunPianApiKey;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConfigManager.SystemConfigInfo.SmsYunPianApiKey = TbApiKey.Text;
                BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

                SuccessMessage("配置短信服务商成功！");

                AddWaitAndRedirectScript(PageSmsProvider.GetRedirectUrl());
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }
        }
    }
}
