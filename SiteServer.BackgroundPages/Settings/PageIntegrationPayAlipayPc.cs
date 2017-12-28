using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using Aop.Api.Util;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageIntegrationPayAlipayPc : BasePage
    {
        public DropDownList DdlIsEnabled;
        public PlaceHolder PhSettings;
        public DropDownList DdlIsMApi;

        public PlaceHolder PhMApi;
        public TextBox TbPid;
        public TextBox TbMd5;

        public PlaceHolder PhOpenApi;
        public TextBox TbAppId;
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

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Integration);

            EBooleanUtils.AddListItems(DdlIsEnabled, "开通", "不开通");
            ControlUtils.SelectSingleItem(DdlIsEnabled, _config.IsAlipayPc.ToString());

            EBooleanUtils.AddListItems(DdlIsMApi, "即时到账（mapi）", "电脑网站支付（openapi）");
            ControlUtils.SelectSingleItem(DdlIsMApi, _config.AlipayPcIsMApi.ToString());

            PhSettings.Visible = _config.IsAlipayPc;

            TbAppId.Text = _config.AlipayPcAppId;
            TbPid.Text = _config.AlipayPcPid;
            TbMd5.Text = _config.AlipayPcMd5;
            TbPublicKey.Text = _config.AlipayPcPublicKey;
            TbPrivateKey.Text = _config.AlipayPcPrivateKey;

            PhMApi.Visible = _config.AlipayPcIsMApi;
            PhOpenApi.Visible = !PhMApi.Visible;
        }

        public void DdlIsEnabled_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhSettings.Visible = TranslateUtils.ToBool(DdlIsEnabled.SelectedValue);
        }

        public void DdlIsMApi_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhMApi.Visible = TranslateUtils.ToBool(DdlIsMApi.SelectedValue);
            PhOpenApi.Visible = !PhMApi.Visible;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            _config.IsAlipayPc = TranslateUtils.ToBool(DdlIsEnabled.SelectedValue);
            if (_config.IsAlipayPc)
            {
                try
                {
                    AlipaySignature.RSASignCharSet("test", TbPrivateKey.Text, "utf-8", false, "RSA2");
                }
                catch (Exception ex)
                {
                    SwalError("应用私钥格式不正确!", ex.Message);
                    return;
                }
            }

            _config.AlipayPcIsMApi = TranslateUtils.ToBool(DdlIsMApi.SelectedValue);
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
