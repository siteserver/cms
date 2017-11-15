using System;
using System.Web;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Plugin.Apis;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageIntegrationPay : BasePage
    {
        public Literal LtlAlipayPc;
        public Literal LtlAlipayMobi;
        public Literal LtlWeixin;
        public Literal LtlJdpay;
        public Literal LtlScript;

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageIntegrationPay), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            if (Body.IsQueryExists("alipayPc"))
            {
                LtlScript.Text = PaymentApi.Instance.ChargeByAlipayPc("测试", 0.01M, StringUtils.GetShortGuid(), "https://www.alipay.com", string.Empty);
            }
            else if (Body.IsQueryExists("alipayMobi"))
            {
                LtlScript.Text = PaymentApi.Instance.ChargeByAlipayMobi("测试", 0.01M, StringUtils.GetShortGuid(), "https://www.alipay.com", string.Empty);
            }
            else if (Body.IsQueryExists("weixin"))
            {
                try
                {
                    var url = HttpUtility.UrlEncode(PaymentApi.Instance.ChargeByWeixin("测试", 0.01M, StringUtils.GetShortGuid(), "https://pay.weixin.qq.com"));
                    LtlScript.Text = $@"<div style=""display: none""><img id=""weixin_test"" src=""{GetRedirectUrl()}?qrcode={url}"" width=""200"" height=""200"" /></div><script>{SwalDom("微信支付测试", "weixin_test")}</script>";
                }
                catch (Exception ex)
                {
                    LtlScript.Text = $"<script>{SwalError("测试报错", ex.Message)}</script>";
                }
            }
            else if (Body.IsQueryExists("qrcode"))
            {
                Response.BinaryWrite(QrCodeUtils.GetBuffer(Request.QueryString["qrcode"]));
                Response.End();
            }
            else if (Body.IsQueryExists("jdpay"))
            {
                LtlScript.Text = PaymentApi.Instance.ChargeByJdpay("测试", 0.01M, StringUtils.GetShortGuid(), "https://www.jdpay.com", string.Empty);
            }

            BreadCrumbSettings("支付设置", AppManager.Permissions.Settings.Integration);

            var config =
                TranslateUtils.JsonDeserialize<IntegrationPayConfig>(
                    ConfigManager.SystemConfigInfo.IntegrationPayConfigJson) ?? new IntegrationPayConfig();

            LtlAlipayPc.Text = config.IsAlipayPc ? $@"
                <span class=""label label-primary"">已开通</span>
                <a class=""m-l-10"" href=""{GetRedirectUrl()}?alipayPc=true"">测试</a>" : "未开通";

            LtlAlipayMobi.Text = config.IsAlipayMobi ? $@"
                <span class=""label label-primary"">已开通</span>
                <a class=""m-l-10"" href=""{GetRedirectUrl()}?alipayMobi=true"">测试</a>" : "未开通";

            LtlWeixin.Text = config.IsWeixin ? $@"
                <span class=""label label-primary"">已开通</span>
                <a class=""m-l-10"" href=""{GetRedirectUrl()}?weixin=true"">测试</a>" : "未开通";

            LtlJdpay.Text = config.IsJdpay ? $@"
                <span class=""label label-primary"">已开通</span>
                <a class=""m-l-10"" href=""{GetRedirectUrl()}?jdpay=true"">测试</a>" : "未开通";
        }
    }
}
