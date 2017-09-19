using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayAcquireRefundResponse.
    /// </summary>
    public class AlipayAcquireRefundResponse : AopResponse
    {
        /// <summary>
        /// 买家支付宝账号，可以是Email或手机号码。
        /// </summary>
        [XmlElement("buyer_logon_id")]
        public string BuyerLogonId { get; set; }

        /// <summary>
        /// 买家支付宝账号对应的支付宝唯一用户号。  以2088开头的纯16位数字
        /// </summary>
        [XmlElement("buyer_user_id")]
        public string BuyerUserId { get; set; }

        /// <summary>
        /// 对返回响应码进行原因说明
        /// </summary>
        [XmlElement("detail_error_code")]
        public string DetailErrorCode { get; set; }

        /// <summary>
        /// 对详细错误码进行文字说明。  当result_code响应码为SUCCESS时，不返回该参数。
        /// </summary>
        [XmlElement("detail_error_des")]
        public string DetailErrorDes { get; set; }

        /// <summary>
        /// 对同一个商户退款请求，如果该笔退款已退款过，则直接返回上一次的退款结果。同时，返回本次请求是否发生了资金变动的标识。  Y：本次退款请求发生资金变动；  N：本次退款请求未发送资金变动。
        /// </summary>
        [XmlElement("fund_change")]
        public string FundChange { get; set; }

        /// <summary>
        /// 对应商户网站的订单系统中的唯一订单号，非支付宝交易号。  需保证在商户网站中的唯一性。是请求时对应的参数，原样返回。
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 退款处理结果响应码。  SUCCESS：退款成功  FAIL：退款失败  UNKNOWN：结果未知
        /// </summary>
        [XmlElement("result_code")]
        public string ResultCode { get; set; }

        /// <summary>
        /// 该交易在支付宝系统中的交易流水号。  最短16位，最长64位。
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }
    }
}
