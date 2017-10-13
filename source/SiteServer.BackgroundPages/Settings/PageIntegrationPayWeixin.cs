using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageIntegrationPayWeixin : BasePage
    {
        public DropDownList DdlIsEnabled;
        public PlaceHolder PhSettings;
        public TextBox TbAppId;
        public TextBox TbAppSecret;
        public TextBox TbMchId;
        public TextBox TbKey;

        private IntegrationPayConfig _config;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageIntegrationPayWeixin), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _config =
                TranslateUtils.JsonDeserialize<IntegrationPayConfig>(
                    ConfigManager.SystemConfigInfo.IntegrationPayConfigJson) ?? new IntegrationPayConfig();

            if (IsPostBack) return;

            BreadCrumbSettings("支付设置", AppManager.Permissions.Settings.Integration);

            EBooleanUtils.AddListItems(DdlIsEnabled, "开通", "不开通");
            ControlUtils.SelectListItems(DdlIsEnabled, _config.IsWeixin.ToString());

            PhSettings.Visible = _config.IsWeixin;

            TbAppId.Text = _config.WeixinAppId;
            TbAppSecret.Text = _config.WeixinAppSecret;
            TbMchId.Text = _config.WeixinMchId;
            TbKey.Text = _config.WeixinKey;
        }

        public void DdlIsEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSettings.Visible = TranslateUtils.ToBool(DdlIsEnabled.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            _config.IsWeixin = TranslateUtils.ToBool(DdlIsEnabled.SelectedValue);
            _config.WeixinAppId = TbAppId.Text;
            _config.WeixinAppSecret = TbAppSecret.Text;
            _config.WeixinMchId = TbMchId.Text;
            _config.WeixinKey = TbKey.Text;

            ConfigManager.SystemConfigInfo.IntegrationPayConfigJson = TranslateUtils.JsonSerialize(_config);
            BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

            PageUtils.Redirect(PageIntegrationPay.GetRedirectUrl());
        }
    }
}
