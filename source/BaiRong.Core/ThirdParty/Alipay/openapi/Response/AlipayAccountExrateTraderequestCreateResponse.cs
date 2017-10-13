using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayAccountExrateTraderequestCreateResponse.
    /// </summary>
    public class AlipayAccountExrateTraderequestCreateResponse : AopResponse
    {
        /// <summary>
        /// 成交汇率的基准币种
        /// </summary>
        [XmlElement("base_ccy")]
        public string BaseCcy { get; set; }

        /// <summary>
        /// 原请求客户号
        /// </summary>
        [XmlElement("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// 对应金额
        /// </summary>
        [XmlElement("contra_amount")]
        public string ContraAmount { get; set; }

        /// <summary>
        /// 对应币种
        /// </summary>
        [XmlElement("contra_ccy")]
        public string ContraCcy { get; set; }

        /// <summary>
        /// FX返回关联该笔业务单据的交易号
        /// </summary>
        [XmlElement("deal_ref")]
        public string DealRef { get; set; }

        /// <summary>
        /// 成交汇率
        /// </summary>
        [XmlElement("dealt_rate")]
        public string DealtRate { get; set; }

        /// <summary>
        /// 该请求是否为重复发送。当为true时，结果码和结果描述，为该交易当前的处理情况。
        /// </summary>
        [XmlElement("duplicate")]
        public string Duplicate { get; set; }

        /// <summary>
        /// 请求类型  字典：H - HedgeAdvise , T - TradeAdvise。
        /// </summary>
        [XmlElement("msg_type")]
        public string MsgType { get; set; }

        /// <summary>
        /// 业务唯一单据号
        /// </summary>
        [XmlElement("requested_message_id")]
        public string RequestedMessageId { get; set; }

        /// <summary>
        /// 汇率使用状态,字典：QUALIFY, EXCEPTION。请求汇率是否被使用，QUALIFY - 与请求汇率一致，EXCEPTION - 未使用请求汇率
        /// </summary>
        [XmlElement("requested_rate_status")]
        public string RequestedRateStatus { get; set; }

        /// <summary>
        /// 请求返回类型，字典，同步受理返回 acknowledge：ACK ;  异步成交回执 executtion  report：EXEC
        /// </summary>
        [XmlElement("response_type")]
        public string ResponseType { get; set; }

        /// <summary>
        /// 交易方向
        /// </summary>
        [XmlElement("side")]
        public string Side { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        [XmlElement("transaction_amount")]
        public string TransactionAmount { get; set; }

        /// <summary>
        /// 交易币种
        /// </summary>
        [XmlElement("transaction_ccy")]
        public string TransactionCcy { get; set; }

        /// <summary>
        /// 起息日
        /// </summary>
        [XmlElement("value_date")]
        public string ValueDate { get; set; }
    }
}
