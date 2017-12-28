using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageIntegrationPayJdpay : BasePage
    {
        public DropDownList DdlIsEnabled;
        public PlaceHolder PhSettings;

        public TextBox TbMerchant;
        public TextBox TbUserId;
        public TextBox TbMd5Key;
        public TextBox TbDesKey;
        public TextBox TbPublicKey;
        public TextBox TbPrivateKey;

        private IntegrationPayConfig _config;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageIntegrationPayJdpay), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _config =
                TranslateUtils.JsonDeserialize<IntegrationPayConfig>(
                    ConfigManager.SystemConfigInfo.IntegrationPayConfigJson) ?? new IntegrationPayConfig();

            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Integration);

            EBooleanUtils.AddListItems(DdlIsEnabled, "开通", "不开通");
            ControlUtils.SelectSingleItem(DdlIsEnabled, _config.IsJdpay.ToString());

            PhSettings.Visible = _config.IsJdpay;

            TbMerchant.Text = _config.JdpayMerchant;
            TbUserId.Text = _config.JdpayUserId;
            TbMd5Key.Text = _config.JdpayMd5Key;
            TbDesKey.Text = _config.JdpayDesKey;
            TbPublicKey.Text = _config.JdpayPublicKey;
            TbPrivateKey.Text = _config.JdpayPrivateKey;
        }

        public void DdlIsEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSettings.Visible = TranslateUtils.ToBool(DdlIsEnabled.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            _config.IsJdpay = TranslateUtils.ToBool(DdlIsEnabled.SelectedValue);
            //if (_config.IsJdpay)
            //{
            //    try
            //    {
            //        AlipaySignature.RSASignCharSet("test", TbPrivateKey.Text, "utf-8", false, "RSA2");
            //    }
            //    catch (Exception ex)
            //    {
            //        SwalError("应用私钥格式不正确!", ex.Message);
            //        return;
            //    }
            //}

            _config.JdpayMerchant = TbMerchant.Text;
            _config.JdpayUserId = TbUserId.Text;
            _config.JdpayMd5Key = TbMd5Key.Text;
            _config.JdpayDesKey = TbDesKey.Text;
            _config.JdpayPublicKey = TbPublicKey.Text;
            _config.JdpayPrivateKey = TbPrivateKey.Text;

            ConfigManager.SystemConfigInfo.IntegrationPayConfigJson = TranslateUtils.JsonSerialize(_config);
            BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

            PageUtils.Redirect(PageIntegrationPay.GetRedirectUrl());
        }
    }
}
