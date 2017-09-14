using System;
using System.Collections.Generic;
using System.Linq;
using BaiRong.Core.Model.Enumerations;
using Pingpp.Models;

namespace BaiRong.Core.Integration
{
    public class PaymentManager
    {
        public static bool IsReady()
        {
            return ConfigManager.SystemConfigInfo.PaymentProviderType == EPaymentProviderType.Pingxx && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.PaymentPingxxAppId) && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.PaymentPingxxSecretKey) && !string.IsNullOrEmpty(ConfigManager.SystemConfigInfo.PaymentChannels);
        }

        public static List<string> GetPaymentChannels()
        {
            return ConfigManager.SystemConfigInfo.PaymentChannels.Split(',').ToList();
        }

        public static object ChargeByPcAlipay(int amount, string orderNo, string successUrl)
        {
            var extra = new Dictionary<string, object>
            {
                {"success_url", successUrl}
            };

            return ChargeByExtra(amount, EPaymentChannel.AlipayPcDirect, orderNo, extra);
        }

        public static object ChargeByWapAlipay(int amount, string orderNo, string successUrl, string cancelUrl)
        {
            var extra = new Dictionary<string, object>
            {
                {"success_url", successUrl},
                {"cancel_url", cancelUrl}
            };

            return ChargeByExtra(amount, EPaymentChannel.AlipayWap, orderNo, extra);
        }

        public static object ChargeByWeixin(int amount, string orderNo, string openId)
        {
            var extra = new Dictionary<string, object>
            {
                {"open_id", openId}
            };

            return ChargeByExtra(amount, EPaymentChannel.WxPub, orderNo, extra);
        }

        private static object ChargeByExtra(int amount, EPaymentChannel channel, string orderNo, Dictionary<string, object> extra)
        {
            Pingpp.Pingpp.SetApiKey(ConfigManager.SystemConfigInfo.PaymentPingxxSecretKey);

            var param = new Dictionary<string, object>
            {
                {"order_no", orderNo},
                {"amount", amount},
                {"channel", EPaymentChannelUtils.GetValue(channel)},
                {"currency", "cny"},
                {"subject", "test"},
                {"body", "tests"},
                {"client_ip", "127.0.0.1"},
                {
                    "app", new Dictionary<string, string>
                    {
                        {"id", ConfigManager.SystemConfigInfo.PaymentPingxxAppId}
                    }
                },
                {"extra", extra}
            };

            return Charge.Create(param);
        }
    }
}