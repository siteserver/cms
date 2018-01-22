using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AdviceVO Data Structure.
    /// </summary>
    [Serializable]
    public class AdviceVO : AopObject
    {
        /// <summary>
        /// 兑换请求发起时间
        /// </summary>
        [XmlElement("client_advice_timestamp")]
        public string ClientAdviceTimestamp { get; set; }

        /// <summary>
        /// 客户号：用于定义FX交易的客户，由外汇交易中心统一分配
        /// </summary>
        [XmlElement("client_id")]
        public string ClientId { get; set; }

        /// <summary>
        /// 客户请求序号:  客户侧的流水号，由客户上送
        /// </summary>
        [XmlElement("client_ref")]
        public string ClientRef { get; set; }

        /// <summary>
        /// 对应金额，选输项
        /// </summary>
        [XmlElement("contra_amount")]
        public string ContraAmount { get; set; }

        /// <summary>
        /// 对应币种
        /// </summary>
        [XmlElement("contra_ccy")]
        public string ContraCcy { get; set; }

        /// <summary>
        /// 端对端流水号  原TransactionID，用于标识全局唯一序号
        /// </summary>
        [XmlElement("end_to_end_id")]
        public string EndToEndId { get; set; }

        /// <summary>
        /// 请求类型：H - HedgeAdvise , T - TradeAdvise。锁价模式下先发送Hedge，在发送对应的Trade。非锁价模式下，可直接发送Trade
        /// </summary>
        [XmlElement("msg_type")]
        public string MsgType { get; set; }

        /// <summary>
        /// 是否部分成交：Y，N。是否可部分成交  默认为不可部分成交
        /// </summary>
        [XmlElement("partial_trade")]
        public string PartialTrade { get; set; }

        /// <summary>
        /// 支付渠道:  上送收单业务中的支付渠道，须提前约定。
        /// </summary>
        [XmlElement("payment_provider")]
        public string PaymentProvider { get; set; }

        /// <summary>
        /// 支付类型, 默认为DEFAULT
        /// </summary>
        [XmlElement("payment_type")]
        public string PaymentType { get; set; }

        /// <summary>
        /// 客户协议扩展号，用于支持同一客户在不同场景下所需的汇率
        /// </summary>
        [XmlElement("profile_id")]
        public string ProfileId { get; set; }

        /// <summary>
        /// 汇率的唯一编码
        /// </summary>
        [XmlElement("rate_ref")]
        public string RateRef { get; set; }

        /// <summary>
        /// 汇率请求模式 :  TA时必输，仅在TA时有效。  字典：REQ - 按客户请求(含滑点)成交，若该价格失效，则交易失败；ACP - 汇率失效或请求中不带汇率，接受该客户协议的最新汇率，实际成交汇率以GlobalFX为准；"
        /// </summary>
        [XmlElement("rate_request_mode")]
        public string RateRequestMode { get; set; }

        /// <summary>
        /// 汇率类型：SPOT，FORWARD。送RateRef的情况下，以RateRef关联的汇率为准。
        /// </summary>
        [XmlElement("rate_type")]
        public string RateType { get; set; }

        /// <summary>
        /// 备用字段1
        /// </summary>
        [XmlElement("reference_field1")]
        public string ReferenceField1 { get; set; }

        /// <summary>
        /// 备用字段2
        /// </summary>
        [XmlElement("reference_field2")]
        public string ReferenceField2 { get; set; }

        /// <summary>
        /// 备用字段3
        /// </summary>
        [XmlElement("reference_field3")]
        public string ReferenceField3 { get; set; }

        /// <summary>
        /// 关联交易请求号: "本次交易请求关联的原交易号。填写规范：  1）正向交易的TA，填写HA MessageId  2）REFUND，添加原SALE的TA MessageId  3）CHARGEBACK，填写原SALE的TA MessageId  4）CHARGEBACK_REVERSE，填写原CHARGEBACK的TA MessageId  5）CANCELLATION，填写被Cancel交易的TA MessageId"
        /// </summary>
        [XmlElement("related_message_id")]
        public string RelatedMessageId { get; set; }

        /// <summary>
        /// 交易请求号
        /// </summary>
        [XmlElement("request_message_id")]
        public string RequestMessageId { get; set; }

        /// <summary>
        /// 客户请求的汇率，送RateRef的情况下，以RateRef关联的汇率为准。
        /// </summary>
        [XmlElement("requested_rate")]
        public string RequestedRate { get; set; }

        /// <summary>
        /// NDF交割下，实际交割币种的金额
        /// </summary>
        [XmlElement("settlement_amount")]
        public string SettlementAmount { get; set; }

        /// <summary>
        /// NDF交割下，实际交割的币种
        /// </summary>
        [XmlElement("settlement_ccy")]
        public string SettlementCcy { get; set; }

        /// <summary>
        /// 买卖方向：BUY,SELL。客户视角对交易货币的操作。该字段为必填，与原TransactionType的对应关系如下：  SALE - SELL  REFUND - BUY  CHARGEBACK - BUY  CHARGEBACK_RESEVSE - SELL  CANCELLATION - 使用原交易的side"
        /// </summary>
        [XmlElement("side")]
        public string Side { get; set; }

        /// <summary>
        /// 汇率上浮滑点 : BP单位，即汇率单位的万分之一。仅在“请求汇率模式”为REQ时有效，在请求汇率/汇率编码对应的汇率的基础上，Side为BUY时上浮滑点，Side为SELL时下浮滑点
        /// </summary>
        [XmlElement("slip_point")]
        public long SlipPoint { get; set; }

        /// <summary>
        /// 调用方事件码
        /// </summary>
        [XmlElement("source_event_code")]
        public string SourceEventCode { get; set; }

        /// <summary>
        /// 调用方产品码
        /// </summary>
        [XmlElement("source_product_code")]
        public string SourceProductCode { get; set; }

        /// <summary>
        /// 时间所在时区
        /// </summary>
        [XmlElement("time_zone")]
        public string TimeZone { get; set; }

        /// <summary>
        /// 上层业务发起时间
        /// </summary>
        [XmlElement("trade_timestamp")]
        public string TradeTimestamp { get; set; }

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
        /// 交易币种交割方式：DELIV，NDF。当Client的实际交割货币与交易币种不一致时，送NDF。
        /// </summary>
        [XmlElement("transaction_ccy_type")]
        public string TransactionCcyType { get; set; }

        /// <summary>
        /// 交易类型:   兼容收单业务兑换使用。字典：SALE，REFUND，CHARGEBACK，CHARGEBACK_REVERSE，CANCELLATION
        /// </summary>
        [XmlElement("transaction_type")]
        public string TransactionType { get; set; }

        /// <summary>
        /// 起息日期 :  YYYYMMDD，客户期望的资金交割日期
        /// </summary>
        [XmlElement("value_date")]
        public string ValueDate { get; set; }
    }
}
