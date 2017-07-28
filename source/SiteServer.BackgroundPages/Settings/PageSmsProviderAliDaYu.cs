using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSmsProviderAliDaYu : BasePage
    {
        public Literal LtlType;

        public TextBox TbAppKey;
        public TextBox TbAppSecret;
        public TextBox TbSignName;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageSmsProviderAliDaYu), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (!IsPostBack)
            {
                BreadCrumbSettings(AppManager.Settings.LeftMenu.Config, "短信服务商管理", AppManager.Settings.Permission.SettingsConfig);

                LtlType.Text = $@"{ESmsProviderTypeUtils.GetText(ESmsProviderType.AliDaYu)}(<a href=""{ESmsProviderTypeUtils.GetUrl(ESmsProviderType.AliDaYu)}"" target=""_blank"">{ESmsProviderTypeUtils.GetUrl(ESmsProviderType.AliDaYu)}</a>)";

                TbAppKey.Text = ConfigManager.SystemConfigInfo.SmsAliDaYuAppKey;
                TbAppSecret.Text = ConfigManager.SystemConfigInfo.SmsAliDaYuAppSecret;
                TbSignName.Text = ConfigManager.SystemConfigInfo.SmsAliDaYuSignName;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            try
            {
                ConfigManager.SystemConfigInfo.SmsAliDaYuAppKey = TbAppKey.Text;
                ConfigManager.SystemConfigInfo.SmsAliDaYuAppSecret = TbAppSecret.Text;
                ConfigManager.SystemConfigInfo.SmsAliDaYuSignName = TbSignName.Text;
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
