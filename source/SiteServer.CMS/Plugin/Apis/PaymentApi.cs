using System;
using Aop.Api;
using Aop.Api.Domain;
using Aop.Api.Request;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.Plugin.Apis;
using WxPayAPI;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace SiteServer.CMS.Plugin.Apis
{
    public class PaymentApi : IPaymentApi
    {
        private PaymentApi() { }

        public static PaymentApi Instance { get; } = new PaymentApi();

        private static IntegrationPayConfig GetConfig()
        {
            return TranslateUtils.JsonDeserialize<IntegrationPayConfig>(
                       ConfigManager.SystemConfigInfo.IntegrationPayConfigJson) ?? new IntegrationPayConfig();
        }

        public bool IsAlipayPc
        {
            get
            {
                var config = GetConfig();
                return config.IsAlipayPc;
            }
        }

        public string ChargeByAlipayPc(string productName, decimal amount, string orderNo, string successUrl)
        {
            var config = GetConfig();
            if (!config.IsAlipayPc) return null;

            if (config.AlipayPcIsMApi)
            {
                // 合作身份者ID，签约账号，以2088开头由16位纯数字组成的字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
                BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.partner = config.AlipayPcPid;

                // 收款支付宝账号，以2088开头由16位纯数字组成的字符串，一般情况下收款账号就是签约账号
                BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.seller_id = BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.partner;

                // MD5密钥，安全检验码，由数字和字母组成的32位字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
                BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.key = config.AlipayPcMd5;

                // 服务器异步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数,必须外网可以正常访问
                BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.notify_url = string.Empty;

                // 页面跳转同步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数，必须外网可以正常访问
                BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.return_url = successUrl;

                // 字符编码格式 目前支持 gbk 或 utf-8
                BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.input_charset = "utf-8";

                // 支付类型 ，无需修改
                BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.payment_type = "1";

                // 调用的接口名，无需修改
                BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.service = "create_direct_pay_by_user";

                ////////////////////////////////////////////////////////////////////////////////////////////////

                //把请求参数打包成数组
                var sParaTemp = new SortedDictionary<string, string>
                {
                    {"service", BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.service},
                    {"partner", BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.partner},
                    {"seller_id", BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.seller_id},
                    {"_input_charset", BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.input_charset.ToLower()},
                    {"payment_type", BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.payment_type},
                    {"notify_url", BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.notify_url},
                    {"return_url", BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.return_url},
                    {"anti_phishing_key", BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.anti_phishing_key},
                    {"exter_invoke_ip", BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Config.exter_invoke_ip},
                    {"out_trade_no", orderNo},
                    {"subject", productName},
                    {"total_fee", amount.ToString("N2")},
                    {"body", string.Empty}
                };
                //商户订单号，商户网站订单系统中唯一订单号，必填
                //其他业务参数根据在线开发文档，添加参数.文档地址:https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.O9yorI&treeId=62&articleId=103740&docType=1
                //如sParaTemp.Add("参数名","参数值");

                //建立请求
                return BaiRong.Core.ThirdParty.Alipay.MApi.Pc.Submit.BuildRequest(sParaTemp, "get", "确认");
            }

            var client = new DefaultAopClient("https://openapi.alipay.com/gateway.do", config.AlipayMobiAppId, config.AlipayMobiPrivateKey, "JSON",
                "1.0", "RSA2", config.AlipayMobiPublicKey, "utf-8", false);

            // 组装业务参数model
            var model = new AlipayTradePagePayModel
            {
                Body = string.Empty,
                Subject = productName,
                TotalAmount = amount.ToString("N2"),
                OutTradeNo = orderNo,
                TimeoutExpress = "90m",
                ProductCode = "FAST_INSTANT_TRADE_PAY"
            };
            // 付款金额
            // 外部订单号，商户网站订单系统中唯一的订单号

            var request = new AlipayTradePagePayRequest();
            // 设置同步回调地址
            request.SetReturnUrl(successUrl);
            // 设置异步通知接收地址
            request.SetNotifyUrl("");
            // 将业务model载入到request
            request.SetBizModel(model);

            var response = client.pageExecute(request);
            return response.Body;
        }

        public bool IsAlipayMobi
        {
            get
            {
                var config = GetConfig();
                return config.IsAlipayMobi;
            }
        }

        public string ChargeByAlipayMobi(string productName, decimal amount, string orderNo, string successUrl)
        {
            var config = GetConfig();
            if (!config.IsAlipayMobi) return null;

            if (config.AlipayMobiIsMApi)
            {
                // 合作身份者ID，签约账号，以2088开头由16位纯数字组成的字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
                BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Config.partner = config.AlipayMobiPid;

                // 收款支付宝账号，以2088开头由16位纯数字组成的字符串，一般情况下收款账号就是签约账号
                BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Config.seller_id = BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Config.partner;

                // MD5密钥，安全检验码，由数字和字母组成的32位字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
                BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Config.key = config.AlipayMobiMd5;

                // 服务器异步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数,必须外网可以正常访问
                BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Config.notify_url = string.Empty;

                // 页面跳转同步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数，必须外网可以正常访问
                BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Config.return_url = successUrl;

                //把请求参数打包成数组
                var sParaTemp = new SortedDictionary<string, string>
                {
                    {"partner", BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Config.partner},
                    {"seller_id", BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Config.seller_id},
                    {"_input_charset", BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Config.input_charset.ToLower()},
                    {"service", BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Config.service},
                    {"payment_type", BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Config.payment_type},
                    {"notify_url", BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Config.notify_url},
                    {"return_url", BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Config.return_url},
                    {"out_trade_no", orderNo},
                    {"subject", productName},
                    {"total_fee", amount.ToString("N2")},
                    {"show_url", successUrl},
                    {"body", string.Empty}
                };
                //商户订单号，商户网站订单系统中唯一订单号，必填
                //收银台页面上，商品展示的超链接，必填
                //sParaTemp.Add("app_pay","Y");//启用此参数可唤起钱包APP支付。
                //其他业务参数根据在线开发文档，添加参数.文档地址:https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.2Z6TSk&treeId=60&articleId=103693&docType=1
                //如sParaTemp.Add("参数名","参数值");

                //建立请求
                return BaiRong.Core.ThirdParty.Alipay.MApi.Mobi.Submit.BuildRequest(sParaTemp, "get", "确认");
            }

            var client = new DefaultAopClient("https://openapi.alipay.com/gateway.do", config.AlipayMobiAppId, config.AlipayMobiPrivateKey, "JSON",
                "1.0", "RSA2", config.AlipayMobiPublicKey, "utf-8", false);

            // 组装业务参数model
            var model = new AlipayTradePagePayModel
            {
                Body = string.Empty,
                Subject = productName,
                TotalAmount = amount.ToString("N2"),
                OutTradeNo = orderNo,
                TimeoutExpress = "90m",
                ProductCode = "QUICK_WAP_PAY"
            };
            // 付款金额
            // 外部订单号，商户网站订单系统中唯一的订单号

            AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
            // 设置同步回调地址
            request.SetReturnUrl(successUrl);
            // 设置异步通知接收地址
            request.SetNotifyUrl("");
            // 将业务model载入到request
            request.SetBizModel(model);

            var response = client.pageExecute(request);
            return response.Body;
        }

        public bool IsWeixin
        {
            get {
                var config = GetConfig();
                return config.IsWeixin;
            }
        }

        public string ChargeByWeixin(string productName, decimal amount, string orderNo, string notifyUrl)
        {
            var config = GetConfig();

            WxPayConfig.APPID = config.WeixinAppId;
            WxPayConfig.MCHID = config.WeixinMchId;
            WxPayConfig.KEY = config.WeixinKey;
            WxPayConfig.APPSECRET = config.WeixinAppSecret;

            //=======【支付结果通知url】===================================== 
            /* 支付结果通知回调url，用于商户接收支付结果
            */
            WxPayConfig.NOTIFY_URL = notifyUrl;

            //=======【商户系统后台机器IP】===================================== 
            /* 此参数可手动配置也可在程序中自动获取
            */
            WxPayConfig.IP = "8.8.8.8";


            //=======【代理服务器设置】===================================
            /* 默认IP和端口号分别为0.0.0.0和0，此时不开启代理（如有需要才设置）
            */
            WxPayConfig.PROXY_URL = "http://10.152.18.220:8080";

            //=======【上报信息配置】===================================
            /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
            */
            WxPayConfig.REPORT_LEVENL = 1;

            //=======【日志级别】===================================
            /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
            */
            WxPayConfig.LOG_LEVENL = 0;

            var data = new WxPayData();
            data.SetValue("body", productName);//商品描述
            data.SetValue("attach", string.Empty);//附加数据
            data.SetValue("out_trade_no", WxPayApi.GenerateOutTradeNo());//随机字符串
            data.SetValue("total_fee", Convert.ToInt32(amount * 100));//总金额
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
            if (!data.IsSet("body"))
            {
                throw new WxPayException("缺少统一支付接口必填参数body！");
            }
            if (!data.IsSet("total_fee"))
            {
                throw new WxPayException("缺少统一支付接口必填参数total_fee！");
            }
            if (!data.IsSet("trade_type"))
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
            var xml = data.ToXml();
            var response = HttpService.Post(xml, url, false, 6);
            var result = new WxPayData();
            result.FromXml(response);

            Log.Info(this.GetType().ToString(), "ChargeByWeixin : " + response);
            Log.Info(this.GetType().ToString(), "notify_url : " + data.GetValue("notify_url"));

            if (!result.IsSet("code_url"))
            {
                throw new Exception($"code: {result.GetValue("return_code")}, msg: {result.GetValue("return_msg")}");
            }
            return result.GetValue("code_url").ToString(); //获得统一下单接口返回的二维码链接
        }

        public void NotifyByWeixin(HttpRequest request, out bool isPaied, out string responseXml)
        {
            isPaied = false;
            var config = GetConfig();

            WxPayConfig.APPID = config.WeixinAppId;
            WxPayConfig.MCHID = config.WeixinMchId;
            WxPayConfig.KEY = config.WeixinKey;
            WxPayConfig.APPSECRET = config.WeixinAppSecret;

            //=======【商户系统后台机器IP】===================================== 
            /* 此参数可手动配置也可在程序中自动获取
            */
            WxPayConfig.IP = "8.8.8.8";


            //=======【代理服务器设置】===================================
            /* 默认IP和端口号分别为0.0.0.0和0，此时不开启代理（如有需要才设置）
            */
            WxPayConfig.PROXY_URL = "http://10.152.18.220:8080";

            //=======【上报信息配置】===================================
            /* 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
            */
            WxPayConfig.REPORT_LEVENL = 1;

            //=======【日志级别】===================================
            /* 日志等级，0.不输出日志；1.只输出错误信息; 2.输出错误和正常信息; 3.输出错误信息、正常信息和调试信息
            */
            WxPayConfig.LOG_LEVENL = 0;

            //接收从微信后台POST过来的数据
            System.IO.Stream s = request.InputStream;
            int count = 0;
            byte[] buffer = new byte[1024];
            StringBuilder builder = new StringBuilder();
            while ((count = s.Read(buffer, 0, 1024)) > 0)
            {
                builder.Append(Encoding.UTF8.GetString(buffer, 0, count));
            }
            s.Flush();
            s.Close();
            s.Dispose();

            Log.Info(this.GetType().ToString(), "NotifyByWeixin : " + builder);

            //转换数据格式并验证签名
            WxPayData notifyData = new WxPayData();
            try
            {
                notifyData.FromXml(builder.ToString());
            }
            catch (WxPayException ex)
            {
                //若签名错误，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", ex.Message);
                Log.Error(this.GetType().ToString(), "Sign check error : " + res.ToXml());
                responseXml = res.ToXml();
                return;
            }

            if (!notifyData.IsSet("return_code") || notifyData.GetValue("return_code").ToString() != "SUCCESS")
            {
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "回调数据异常");
                Log.Info(this.GetType().ToString(), "The data WeChat post is error : " + res.ToXml());
                responseXml = res.ToXml();
                return;
            }

            //统一下单成功,则返回成功结果给微信支付后台
            WxPayData data = new WxPayData();
            data.SetValue("return_code", "SUCCESS");
            data.SetValue("return_msg", "OK");

            Log.Info(this.GetType().ToString(), "UnifiedOrder success , send data to WeChat : " + data.ToXml());
            isPaied = true;
            responseXml = data.ToXml();
        }

        public bool IsUnionpayPc
        {
            get
            {
                var config = GetConfig();
                return config.IsUnionpayPc;
            }
        }

        public bool IsUnionpayMobi
        {
            get
            {
                var config = GetConfig();
                return config.IsUnionpayMobi;
            }
        }
    }
}

