using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using Aop.Api.Util;
using SiteServer.CMS.Plugin.Apis;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageIntegrationPayAlipayPc : BasePage
    {
        public DropDownList DdlIsEnabled;
        public PlaceHolder PhSettings;
        public TextBox TbAccount;
        public TextBox TbAppId;
        public TextBox TbPid;
        public TextBox TbMd5;
        public TextBox TbPublicKey;
        public TextBox TbPrivateKey;

        private IntegrationPayConfig _config;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageIntegrationPayAlipayPc), null);
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
            ControlUtils.SelectListItems(DdlIsEnabled, _config.IsAlipayPc.ToString());

            PhSettings.Visible = _config.IsAlipayPc;

            TbAccount.Text = _config.AlipayPcAccount;
            TbAppId.Text = _config.AlipayPcAppId;
            TbPid.Text = _config.AlipayPcPid;
            TbMd5.Text = _config.AlipayPcMd5;
            TbPublicKey.Text = _config.AlipayPcPublicKey;
            TbPrivateKey.Text = _config.AlipayPcPrivateKey;
        }

        public void DdlIsEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSettings.Visible = TranslateUtils.ToBool(DdlIsEnabled.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            _config.IsAlipayPc = TranslateUtils.ToBool(DdlIsEnabled.SelectedValue);
            if (_config.IsAlipayPc)
            {
                try
                {
                    AlipaySignature.RSASignCharSet("test", TbPrivateKey.Text, PaymentApi.AlipayCharset,
                        false, PaymentApi.AlipaySignType);
                }
                catch (Exception ex)
                {
                    SwalError("应用私钥格式不正确!", ex.Message);
                    return;
                }
            }
            _config.AlipayPcAccount = TbAccount.Text;
            _config.AlipayPcAppId = TbAppId.Text;
            _config.AlipayPcPid = TbPid.Text;
            _config.AlipayPcMd5 = TbMd5.Text;
            _config.AlipayPcPublicKey = TbPublicKey.Text;
            _config.AlipayPcPrivateKey = TbPrivateKey.Text;

            ConfigManager.SystemConfigInfo.IntegrationPayConfigJson = TranslateUtils.JsonSerialize(_config);
            BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

            PageUtils.Redirect(PageIntegrationPay.GetRedirectUrl());
        }
    }
}
