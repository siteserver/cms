using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayTradePayResponse.
    /// </summary>
    public class AlipayTradePayResponse : AopResponse
    {
        /// <summary>
        /// 异步支付模式，先享后付业务会返回该参数，目前有三种值：  ASYNC_DELAY_PAY(异步延时付款);  ASYNC_REALTIME_PAY(异步准实时付款);  SYNC_DIRECT_PAY(同步直接扣款);
        /// </summary>
        [XmlElement("async_payment_mode")]
        public string AsyncPaymentMode { get; set; }

        /// <summary>
        /// 买家支付宝账号
        /// </summary>
        [XmlElement("buyer_logon_id")]
        public string BuyerLogonId { get; set; }

        /// <summary>
        /// 买家付款的金额
        /// </summary>
        [XmlElement("buyer_pay_amount")]
        public string BuyerPayAmount { get; set; }

        /// <summary>
        /// 买家在支付宝的用户id
        /// </summary>
        [XmlElement("buyer_user_id")]
        public string BuyerUserId { get; set; }

        /// <summary>
        /// 支付宝卡余额
        /// </summary>
        [XmlElement("card_balance")]
        public string CardBalance { get; set; }

        /// <summary>
        /// 本次交易支付所使用的单品券优惠的商品优惠信息
        /// </summary>
        [XmlElement("discount_goods_detail")]
        public string DiscountGoodsDetail { get; set; }

        /// <summary>
        /// 交易支付使用的资金渠道
        /// </summary>
        [XmlArray("fund_bill_list")]
        [XmlArrayItem("trade_fund_bill")]
        public List<TradeFundBill> FundBillList { get; set; }

        /// <summary>
        /// 交易支付时间
        /// </summary>
        [XmlElement("gmt_payment")]
        public string GmtPayment { get; set; }

        /// <summary>
        /// 交易中可给用户开具发票的金额
        /// </summary>
        [XmlElement("invoice_amount")]
        public string InvoiceAmount { get; set; }

        /// <summary>
        /// 买家支付宝用户号,该参数已废弃，请不要使用
        /// </summary>
        [XmlElement("open_id")]
        public string OpenId { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 使用积分宝付款的金额
        /// </summary>
        [XmlElement("point_amount")]
        public string PointAmount { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>
        [XmlElement("receipt_amount")]
        public string ReceiptAmount { get; set; }

        /// <summary>
        /// 发生支付交易的商户门店名称
        /// </summary>
        [XmlElement("store_name")]
        public string StoreName { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        [XmlElement("total_amount")]
        public string TotalAmount { get; set; }

        /// <summary>
        /// 支付宝交易号
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }

        /// <summary>
        /// 本交易支付时使用的所有优惠券信息
        /// </summary>
        [XmlArray("voucher_detail_list")]
        [XmlArrayItem("voucher_detail")]
        public List<VoucherDetail> VoucherDetailList { get; set; }
    }
}
