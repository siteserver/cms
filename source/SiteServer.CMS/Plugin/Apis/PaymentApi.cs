using BaiRong.Core;
using BaiRong.Core.Integration;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin.Apis;

namespace SiteServer.CMS.Plugin.Apis
{
    public class PaymentApi : IPaymentApi
    {
        private PaymentApi() { }

        public static PaymentApi Instance { get; } = new PaymentApi();

        public bool IsReady()
        {
            return PaymentManager.IsReady();
        }

        public bool IsPcAlipay()
        {
            return IsReady() &&
                   StringUtils.In(ConfigManager.SystemConfigInfo.PaymentChannels,
                       EPaymentChannelUtils.GetValue(EPaymentChannel.AlipayPcDirect));
        }

        public object ChargeByPcAlipay(int amount, string orderNo, string successUrl)
        {
            return PaymentManager.ChargeByPcAlipay(amount, orderNo, successUrl);
        }

        public bool IsWapAlipay()
        {
            return IsReady() &&
                   StringUtils.In(ConfigManager.SystemConfigInfo.PaymentChannels,
                       EPaymentChannelUtils.GetValue(EPaymentChannel.AlipayWap));
        }

        public object ChargeByWapAlipay(int amount, string orderNo, string successUrl, string cancelUrl)
        {
            return PaymentManager.ChargeByWapAlipay(amount, orderNo, successUrl, cancelUrl);
        }

        public bool IsPcWeixin()
        {
            return IsReady() &&
                   StringUtils.In(ConfigManager.SystemConfigInfo.PaymentChannels,
                       EPaymentChannelUtils.GetValue(EPaymentChannel.WxPub));
        }

        public object ChargeByPcWeixin(int amount, string orderNo, string openId)
        {
            return PaymentManager.ChargeByWeixin(amount, orderNo, openId);
        }

        public bool IsWapWeiXin()
        {
            return IsReady() &&
                   StringUtils.In(ConfigManager.SystemConfigInfo.PaymentChannels,
                       EPaymentChannelUtils.GetValue(EPaymentChannel.WxPub));
        }

        public object ChargeByWapWeiXin(int amount, string orderNo, string openId)
        {
            return PaymentManager.ChargeByWeixin(amount, orderNo, openId);
        }

        public bool IsPcUnionPay()
        {
            return IsReady() &&
                   StringUtils.In(ConfigManager.SystemConfigInfo.PaymentChannels,
                       EPaymentChannelUtils.GetValue(EPaymentChannel.UpacpPc));
        }

        public bool IsWapUnionPay()
        {
            return IsReady() &&
                   StringUtils.In(ConfigManager.SystemConfigInfo.PaymentChannels,
                       EPaymentChannelUtils.GetValue(EPaymentChannel.UpacpWap));
        }

        public object ChargeByPcUnionPay(int amount, string orderNo, string successUrl)
        {
            throw new System.NotImplementedException();
        }

        public object ChargeByWapUnionPay(int amount, string orderNo, string successUrl)
        {
            throw new System.NotImplementedException();
        }
    }
}
