using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Plugin.Apis;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageIntegrationPay : BasePage
    {
        public Literal LtlAlipayPc;
        public Literal LtlAlipayMobi;
        public Literal LtlWeixin;
        public Literal LtlUnionpayPc;
        public Literal LtlUnionpayMobi;
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
                LtlScript.Text = PaymentApi.Instance.ChargeByAlipayPc(0.01M, "", "").ToString();
            }
            else if (Body.IsQueryExists("weixin"))
            {
                try
                {
                    var url = HttpUtility.UrlEncode(PaymentApi.Instance.ChargeByWeixin(0.01M, "", "").ToString());
                    LtlScript.Text = $@"<img src=""{GetRedirectUrl()}?qrcode={url}"" width=""200"" height=""200"" />";
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

            BreadCrumbSettings("支付设置", AppManager.Permissions.Settings.Integration);

            var config =
                TranslateUtils.JsonDeserialize<IntegrationPayConfig>(
                    ConfigManager.SystemConfigInfo.IntegrationPayConfigJson) ?? new IntegrationPayConfig();

            LtlAlipayPc.Text = config.IsAlipayPc ? $@"<span class=""label label-primary"">已开通</span><a class=""m-l-10"" href=""{GetRedirectUrl()}?alipayPc=true"">测试</a>" : "未开通";
            LtlAlipayMobi.Text = config.IsAlipayMobi ? @"<span class=""label label-primary"">已开通</span>" : "未开通";
            LtlWeixin.Text = config.IsWeixin ? $@"<span class=""label label-primary"">已开通</span><a class=""m-l-10"" href=""{GetRedirectUrl()}?weixin=true"">测试</a>" : "未开通";
            LtlUnionpayPc.Text = config.IsUnionpayPc ? @"<span class=""label label-primary"">已开通</span>" : "未开通";
            LtlUnionpayMobi.Text = config.IsUnionpayMobi ? @"<span class=""label label-primary"">已开通</span>" : "未开通";
        }
    }
}
