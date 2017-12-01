using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayAccountExrateAdviceAcceptResponse.
    /// </summary>
    public class AlipayAccountExrateAdviceAcceptResponse : AopResponse
    {
        /// <summary>
        /// 客户号：用于定义FX交易的客户，由外汇交易中心统一分配
        /// </summary>
        [XmlElement("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// 对应金额，选输项
        /// </summary>
        [XmlElement("contra_amount")]
        public string ContraAmount { get; set; }

        /// <summary>
        /// 相应币种
        /// </summary>
        [XmlElement("contra_ccy")]
        public string ContraCcy { get; set; }

        /// <summary>
        /// FX中心的处理序号
        /// </summary>
        [XmlElement("deal_ref")]
        public string DealRef { get; set; }

        /// <summary>
        /// 实际成交的汇率，原FXRateUsed。
        /// </summary>
        [XmlElement("dealt_rate")]
        public string DealtRate { get; set; }

        /// <summary>
        /// 是否重复消息
        /// </summary>
        [XmlElement("duplicate")]
        public bool Duplicate { get; set; }

        /// <summary>
        /// 交易请求号
        /// </summary>
        [XmlElement("messag_id")]
        public string MessagId { get; set; }

        /// <summary>
        /// 请求类型  字典：H - HedgeAdvise , T - TradeAdvise。锁价模式下先发送Hedge，在发送对应的Trade。非锁价模式下，可直接发送Trade
        /// </summary>
        [XmlElement("msg_type")]
        public string MsgType { get; set; }

        /// <summary>
        /// 汇率使用状态  字典：QUALIFY, EXCEPTION。请求汇率是否被使用，QUALIFY - 与请求汇率一致，EXCEPTION - 未使用请求汇率
        /// </summary>
        [XmlElement("requested_rate_status")]
        public string RequestedRateStatus { get; set; }

        /// <summary>
        /// 买卖方向：BUY,SELL。客户视角对交易货币的操作。该字段为必填，与原TransactionType的对应关系如下：  SALE - SELL  REFUND - BUY  CHARGEBACK - BUY  CHARGEBACK_RESEVSE - SELL  CANCELLATION - 使用原交易的side"  r1
        /// </summary>
        [XmlElement("side")]
        public string Side { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        [XmlElement("transaction_amount")]
        public string TransactionAmount { get; set; }

        /// <summary>
        /// 交易币种: 客户视角的交易买卖币种
        /// </summary>
        [XmlElement("transaction_ccy")]
        public string TransactionCcy { get; set; }

        /// <summary>
        /// 起息日期 :  YYYYMMDD，客户期望的资金交割日期
        /// </summary>
        [XmlElement("value_date")]
        public string ValueDate { get; set; }
    }
}
