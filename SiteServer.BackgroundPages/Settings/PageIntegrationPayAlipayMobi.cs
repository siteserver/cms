using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using Aop.Api.Util;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageIntegrationPayAlipayMobi : BasePage
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
            return PageUtils.GetSettingsUrl(nameof(PageIntegrationPayAlipayMobi), null);
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
            ControlUtils.SelectSingleItem(DdlIsEnabled, _config.IsAlipayMobi.ToString());

            EBooleanUtils.AddListItems(DdlIsMApi, "手机网站支付（mapi）", "手机网站支付（openapi）");
            ControlUtils.SelectSingleItem(DdlIsMApi, _config.AlipayMobiIsMApi.ToString());

            PhSettings.Visible = _config.IsAlipayMobi;

            TbAppId.Text = _config.AlipayMobiAppId;
            TbPid.Text = _config.AlipayMobiPid;
            TbMd5.Text = _config.AlipayMobiMd5;
            TbPublicKey.Text = _config.AlipayMobiPublicKey;
            TbPrivateKey.Text = _config.AlipayMobiPrivateKey;

            PhMApi.Visible = _config.AlipayMobiIsMApi;
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
            _config.IsAlipayMobi = TranslateUtils.ToBool(DdlIsEnabled.SelectedValue);
            if (_config.IsAlipayMobi)
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

            _config.AlipayMobiIsMApi = TranslateUtils.ToBool(DdlIsMApi.SelectedValue);
            _config.AlipayMobiAppId = TbAppId.Text;
            _config.AlipayMobiPid = TbPid.Text;
            _config.AlipayMobiMd5 = TbMd5.Text;
            _config.AlipayMobiPublicKey = TbPublicKey.Text;
            _config.AlipayMobiPrivateKey = TbPrivateKey.Text;

            ConfigManager.SystemConfigInfo.IntegrationPayConfigJson = TranslateUtils.JsonSerialize(_config);
            BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);

            PageUtils.Redirect(PageIntegrationPay.GetRedirectUrl());
        }
    }
}
