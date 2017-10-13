using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayAcquireCancelResponse.
    /// </summary>
    public class AlipayAcquireCancelResponse : AopResponse
    {
        /// <summary>
        /// 对返回响应码进行原因说明，当result_code响应码为SUCCESS时，不返回该参数
        /// </summary>
        [XmlElement("detail_error_code")]
        public string DetailErrorCode { get; set; }

        /// <summary>
        /// 对详细错误码进行文字说明。  当result_code响应码为SUCCESS时，不返回该参数
        /// </summary>
        [XmlElement("detail_error_des")]
        public string DetailErrorDes { get; set; }

        /// <summary>
        /// 对应商户网站的订单系统中的唯一订单号，非支付宝交易号。  需保证在商户网站中的唯一性。是请求时对应的参数，原样返回。
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 撤销处理结果响应码。  SUCCESS：撤销成功  FAIL：撤销失败  UNKNOWN：结果未知
        /// </summary>
        [XmlElement("result_code")]
        public string ResultCode { get; set; }

        /// <summary>
        /// 对撤销失败的情况下，是否可以继续发起撤销请求的建议。  Y：可继续发起撤销请求；  N：不可继续发起撤销请求，即后续的撤销请求也不会成功。
        /// </summary>
        [XmlElement("retry_flag")]
        public string RetryFlag { get; set; }

        /// <summary>
        /// 该交易在支付宝系统中的交易流水号。  最短16位，最长64位。
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }
    }
}
