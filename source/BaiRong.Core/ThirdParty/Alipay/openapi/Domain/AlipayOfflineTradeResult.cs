using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOfflineTradeResult Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOfflineTradeResult : AopObject
    {
        /// <summary>
        /// 系统异常
        /// </summary>
        [XmlElement("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 错误信息描述
        /// </summary>
        [XmlElement("error_message")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 脱机交易处理结果描述
        /// </summary>
        [XmlElement("message")]
        public string Message { get; set; }

        /// <summary>
        /// 表示是否需要重试
        /// </summary>
        [XmlElement("need_retry")]
        public bool NeedRetry { get; set; }

        /// <summary>
        /// 交易需要重试时下一次重试时间
        /// </summary>
        [XmlElement("next_try_time")]
        public string NextTryTime { get; set; }

        /// <summary>
        /// 支付宝外部交易号
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 业务处理结果,SUCCESS：处理成功，FAIL：处理失败， UNKNOWN:结果未知。当结果非SUCCESS时，检查need_retry判断是否需要重试。
        /// </summary>
        [XmlElement("result")]
        public string Result { get; set; }

        /// <summary>
        /// 支付宝交易号
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }
    }
}
