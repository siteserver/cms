using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayAcquireCreateandpayResponse.
    /// </summary>
    public class AlipayAcquireCreateandpayResponse : AopResponse
    {
        /// <summary>
        /// 买家支付宝账号，可以为email或者手机号。对部分信息进行了隐藏。
        /// </summary>
        [XmlElement("buyer_logon_id")]
        public string BuyerLogonId { get; set; }

        /// <summary>
        /// 买家支付宝账号对应的支付宝唯一用户号。  以2088开头的纯16位数字。
        /// </summary>
        [XmlElement("buyer_user_id")]
        public string BuyerUserId { get; set; }

        /// <summary>
        /// 对返回响应码进行原因说明，请参见“10.2 业务错误码”。  当result_code响应码为ORDER_SUCCESS_PAY_SUCCESS时，不返回该参数。
        /// </summary>
        [XmlElement("detail_error_code")]
        public string DetailErrorCode { get; set; }

        /// <summary>
        /// 对详细错误码进行文字说明。  当result_code响应码为ORDER_SUCCESS_PAY_SUCCESS时，不返回该参数。
        /// </summary>
        [XmlElement("detail_error_des")]
        public string DetailErrorDes { get; set; }

        /// <summary>
        /// 支付后返回的其他信息，如预付卡金额，key值mcard_fee，以Json格式返回。
        /// </summary>
        [XmlElement("extend_info")]
        public string ExtendInfo { get; set; }

        /// <summary>
        /// 7085502131376415
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 下单并支付处理结果响应码，请参见“10.1 业务响应码”。
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
