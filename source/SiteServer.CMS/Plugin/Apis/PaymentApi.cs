using System;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using Aop.Api.Response;
using BaiRong.Core;
using BaiRong.Core.Integration;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin.Apis;
using WxPayAPI;
using System.Collections.Generic;

namespace SiteServer.CMS.Plugin.Apis
{
    public class PaymentApi : IPaymentApi
    {
        private PaymentApi() { }

        public static PaymentApi Instance { get; } = new PaymentApi();

        // 支付宝网关
        private const string AlipayGatewayUrl = "https://openapi.alipay.com/gateway.do";
        // 签名方式
        public const string AlipaySignType = "RSA2";
        // 编码格式
        public const string AlipayCharset = "UTF-8";

        private static IntegrationPayConfig GetConfig()
        {
            return TranslateUtils.JsonDeserialize<IntegrationPayConfig>(
                       ConfigManager.SystemConfigInfo.IntegrationPayConfigJson) ?? new IntegrationPayConfig();
        }

        public bool IsAlipayPc()
        {
            var config = GetConfig();
            return config.IsAlipayPc;
        }

        public object ChargeByAlipayPc(decimal amount, string orderNo, string successUrl)
        {
            var config = GetConfig();

            var client = new DefaultAopClient(AlipayGatewayUrl, config.AlipayPcAppId, config.AlipayPcPrivateKey, "json",
                "1.0", AlipaySignType, config.AlipayPcPublicKey, AlipayCharset, false);

            // 组装业务参数model
            var model = new AlipayTradePagePayModel
            {
                Body = "商品描述",
                Subject = "订单名称",
                TotalAmount = amount.ToString("N2"),
                OutTradeNo = orderNo,
                ProductCode = "FAST_INSTANT_TRADE_PAY"
            };
            // 付款金额
            // 外部订单号，商户网站订单系统中唯一的订单号

            AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();
            // 设置同步回调地址
            request.SetReturnUrl(successUrl);
            // 设置异步通知接收地址
            request.SetNotifyUrl("");
            // 将业务model载入到request
            request.SetBizModel(model);

            var response = client.pageExecute(request, null, "post");
            return response.Body;
        }

        public bool IsWeixin()
        {
            var config = GetConfig();
            return config.IsWeixin;
        }

        public object ChargeByWeixin(decimal amount, string orderNo, string successUrl)
        {
            var config = GetConfig();

            WxPayData data = new WxPayData();
            data.SetValue("body", "test");//商品描述
            data.SetValue("attach", "test");//附加数据
            data.SetValue("out_trade_no", WxPayApi.GenerateOutTradeNo());//随机字符串
            data.SetValue("total_fee", 1);//总金额
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));//交易起始时间
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));//交易结束时间
            data.SetValue("goods_tag", "jjj");//商品标记
            data.SetValue("trade_type", "NATIVE");//交易类型
            data.SetValue("product_id", "productId");//商品ID

            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            //检测必填参数
            if (!data.IsSet("out_trade_no"))
            {
                throw new WxPayException("缺少统一支付接口必填参数out_trade_no！");
            }
            else if (!data.IsSet("body"))
            {
                throw new WxPayException("缺少统一支付接口必填参数body！");
            }
            else if (!data.IsSet("total_fee"))
            {
                throw new WxPayException("缺少统一支付接口必填参数total_fee！");
            }
            else if (!data.IsSet("trade_type"))
            {
                throw new WxPayException("缺少统一支付接口必填参数trade_type！");
            }

            //关联参数
            if (data.GetValue("trade_type").ToString() == "JSAPI" && !data.IsSet("openid"))
            {
                throw new WxPayException("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
            }
            if (data.GetValue("trade_type").ToString() == "NATIVE" && !data.IsSet("product_id"))
            {
                throw new WxPayException("统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
            }

            //异步通知url未设置，则使用配置文件中的url
            if (!data.IsSet("notify_url"))
            {
                data.SetValue("notify_url", WxPayConfig.NOTIFY_URL);//异步通知url
            }

            data.SetValue("appid", WxPayConfig.APPID);//公众账号ID
            data.SetValue("mch_id", WxPayConfig.MCHID);//商户号
            data.SetValue("spbill_create_ip", WxPayConfig.IP);//终端ip	  	    
            data.SetValue("nonce_str", Guid.NewGuid().ToString().Replace("-", ""));//随机字符串

            //签名
            data.SetValue("sign", data.MakeSign());
            string xml = data.ToXml();

            var start = DateTime.Now;

            Log.Debug("WxPayApi", "UnfiedOrder request : " + xml);
            string response = HttpService.Post(xml, url, false, 6);
            Log.Debug("WxPayApi", "UnfiedOrder response : " + response);

            var end = DateTime.Now;
            int timeCost = (int)((end - start).TotalMilliseconds);

            WxPayData result = new WxPayData();
            result.FromXml(response);

            //

            if (result.IsSet("code_url"))
            {
                return result.GetValue("code_url").ToString(); //获得统一下单接口返回的二维码链接
            }
            else
            {
                throw new Exception($"code: {result.GetValue("return_code")}, msg: {result.GetValue("return_msg")}");
            }
        }
    }
}

