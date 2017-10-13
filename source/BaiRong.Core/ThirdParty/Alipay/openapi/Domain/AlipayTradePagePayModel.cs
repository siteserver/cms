using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayTradePagePayModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayTradePagePayModel : AopObject
    {
        /// <summary>
        /// 签约参数，支付后签约场景使用
        /// </summary>
        [XmlElement("agreement_sign_params")]
        public AgreementSignParams AgreementSignParams { get; set; }

        /// <summary>
        /// 订单描述
        /// </summary>
        [XmlElement("body")]
        public string Body { get; set; }

        /// <summary>
        /// 禁用渠道,用户不可用指定渠道支付  注，与enable_pay_channels互斥
        /// </summary>
        [XmlElement("disable_pay_channels")]
        public string DisablePayChannels { get; set; }

        /// <summary>
        /// 可用渠道,用户只能在指定渠道范围内支付  注，与disable_pay_channels互斥
        /// </summary>
        [XmlElement("enable_pay_channels")]
        public string EnablePayChannels { get; set; }

        /// <summary>
        /// 业务扩展参数
        /// </summary>
        [XmlElement("extend_params")]
        public ExtendParams ExtendParams { get; set; }

        /// <summary>
        /// 订单包含的商品列表信息，Json格式，其它说明详见商品明细说明
        /// </summary>
        [XmlArray("goods_detail")]
        [XmlArrayItem("goods_detail")]
        public List<GoodsDetail> GoodsDetail { get; set; }

        /// <summary>
        /// 商品主类型 :0-虚拟类商品,1-实物类商品  注：虚拟类商品不支持使用花呗渠道
        /// </summary>
        [XmlElement("goods_type")]
        public string GoodsType { get; set; }

        /// <summary>
        /// 请求后页面的集成方式。  取值范围：  1. ALIAPP：支付宝钱包内  2. PCWEB：PC端访问  默认值为PCWEB。
        /// </summary>
        [XmlElement("integration_type")]
        public string IntegrationType { get; set; }

        /// <summary>
        /// 开票信息
        /// </summary>
        [XmlElement("invoice_info")]
        public InvoiceInfo InvoiceInfo { get; set; }

        /// <summary>
        /// 商户订单号,64个字符以内、可包含字母、数字、下划线；需保证在商户端不重复
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 公用回传参数，如果请求时传递了该参数，则返回给商户时会回传该参数。支付宝只会在同步返回（包括跳转回商户网站）和异步通知时将该参数原样返回。本参数必须进行UrlEncode之后才可以发送给支付宝。
        /// </summary>
        [XmlElement("passback_params")]
        public string PassbackParams { get; set; }

        /// <summary>
        /// 销售产品码，与支付宝签约的产品码名称。  注：目前仅支持FAST_INSTANT_TRADE_PAY
        /// </summary>
        [XmlElement("product_code")]
        public string ProductCode { get; set; }

        /// <summary>
        /// 优惠参数  注：仅与支付宝协商后可用
        /// </summary>
        [XmlElement("promo_params")]
        public string PromoParams { get; set; }

        /// <summary>
        /// PC扫码支付的方式，支持前置模式和    跳转模式。  前置模式是将二维码前置到商户  的订单确认页的模式。需要商户在  自己的页面中以 iframe 方式请求  支付宝页面。具体分为以下几种：  0：订单码-简约前置模式，对应 iframe 宽度不能小于600px，高度不能小于300px；  1：订单码-前置模式，对应iframe 宽度不能小于 300px，高度不能小于600px；  3：订单码-迷你前置模式，对应 iframe 宽度不能小于 75px，高度不能小于75px；  4：订单码-可定义宽度的嵌入式二维码，商户可根据需要设定二维码的大小。    跳转模式下，用户的扫码界面是由支付宝生成的，不在商户的域名下。  2：订单码-跳转模式
        /// </summary>
        [XmlElement("qr_pay_mode")]
        public string QrPayMode { get; set; }

        /// <summary>
        /// 商户自定义二维码宽度  注：qr_pay_mode=4时该参数生效
        /// </summary>
        [XmlElement("qrcode_width")]
        public long QrcodeWidth { get; set; }

        /// <summary>
        /// 请求来源地址。如果使用ALIAPP的集成方式，用户中途取消支付会返回该地址。
        /// </summary>
        [XmlElement("request_from_url")]
        public string RequestFromUrl { get; set; }

        /// <summary>
        /// 描述分账信息，Json格式，详见分账参数说明
        /// </summary>
        [XmlElement("royalty_info")]
        public RoyaltyInfo RoyaltyInfo { get; set; }

        /// <summary>
        /// 商户门店编号
        /// </summary>
        [XmlElement("store_id")]
        public string StoreId { get; set; }

        /// <summary>
        /// 间连受理商户信息体，当前只对特殊银行机构特定场景下使用此字段
        /// </summary>
        [XmlElement("sub_merchant")]
        public SubMerchant SubMerchant { get; set; }

        /// <summary>
        /// 订单标题
        /// </summary>
        [XmlElement("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// 绝对超时时间，格式为yyyy-MM-dd HH:mm
        /// </summary>
        [XmlElement("time_expire")]
        public string TimeExpire { get; set; }

        /// <summary>
        /// 该笔订单允许的最晚付款时间，逾期将关闭交易。取值范围：1m～15d。m-分钟，h-小时，d-天，1c-当天（1c-当天的情况下，无论交易何时创建，都在0点关闭）。 该参数数值不接受小数点， 如 1.5h，可转换为 90m
        /// </summary>
        [XmlElement("timeout_express")]
        public string TimeoutExpress { get; set; }

        /// <summary>
        /// 订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]。
        /// </summary>
        [XmlElement("total_amount")]
        public string TotalAmount { get; set; }
    }
}
