using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InvoiceTradeInfo Data Structure.
    /// </summary>
    [Serializable]
    public class InvoiceTradeInfo : AopObject
    {
        /// <summary>
        /// 支付宝交易号（字段于2017-02-21废弃，请勿使用）
        /// </summary>
        [XmlElement("alipay_trade_no")]
        public string AlipayTradeNo { get; set; }

        /// <summary>
        /// 交易创建时间 yyyy-mm-dd hh:mm:ss
        /// </summary>
        [XmlElement("create_trade_date")]
        public string CreateTradeDate { get; set; }

        /// <summary>
        /// 交易号（发票管家指定）
        /// </summary>
        [XmlElement("einv_trade_id")]
        public string EinvTradeId { get; set; }

        /// <summary>
        /// 交易商品总称
        /// </summary>
        [XmlElement("goods_name")]
        public string GoodsName { get; set; }

        /// <summary>
        /// 发票内容项明细
        /// </summary>
        [XmlElement("invoice_content")]
        public InvoiceItemQueryOpenModel InvoiceContent { get; set; }

        /// <summary>
        /// 品牌全称，由商户在发票管家配置
        /// </summary>
        [XmlElement("m_name")]
        public string MName { get; set; }

        /// <summary>
        /// 交易商户品牌简称
        /// </summary>
        [XmlElement("m_short_name")]
        public string MShortName { get; set; }

        /// <summary>
        /// 交易所属的商户id，即卖家主体标志，可以为支付宝的门店id  也可以为支付宝的签约pid，也可以为支付宝的收款账户seller_user_id
        /// </summary>
        [XmlElement("merchant_id")]
        public string MerchantId { get; set; }

        /// <summary>
        /// 商户交易订单号
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 交易支付时间 yyyy-mm-dd hh:mm:ss
        /// </summary>
        [XmlElement("payment_trade_date")]
        public string PaymentTradeDate { get; set; }

        /// <summary>
        /// 实际付款金额，不包含商户优惠金额
        /// </summary>
        [XmlElement("real_amount")]
        public string RealAmount { get; set; }

        /// <summary>
        /// 商户门店全称，由商户在发票管家配置
        /// </summary>
        [XmlElement("sub_m_name")]
        public string SubMName { get; set; }

        /// <summary>
        /// 商户交易门店简称，一般由m_short_name+sub_m_short_name确定唯一的商户，这两项配置需要商户提前在支付宝配置
        /// </summary>
        [XmlElement("sub_m_short_name")]
        public string SubMShortName { get; set; }

        /// <summary>
        /// 交易总金额，精确到小数点两位，以元为单位
        /// </summary>
        [XmlElement("trade_amount")]
        public string TradeAmount { get; set; }

        /// <summary>
        /// 交易资金明细列表
        /// </summary>
        [XmlArray("trade_fund_list")]
        [XmlArrayItem("invoice_trade_fund_item")]
        public List<InvoiceTradeFundItem> TradeFundList { get; set; }

        /// <summary>
        /// 交易商品明细列表
        /// </summary>
        [XmlArray("trade_goods_list")]
        [XmlArrayItem("invoice_trade_goods_item")]
        public List<InvoiceTradeGoodsItem> TradeGoodsList { get; set; }

        /// <summary>
        /// 支付宝交易号
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }

        /// <summary>
        /// 交易的买家支付宝账户id
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}
