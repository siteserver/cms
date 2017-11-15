using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayAcquireQueryResponse.
    /// </summary>
    public class AlipayAcquireQueryResponse : AopResponse
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
        /// 对返回响应码进行原因说明”。  当result_code响应码为SUCCESS时，不返回该参数
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
        /// 签约的支付宝账号对应的支付宝唯一用户号。  以2088开头的16位纯数字组成。
        /// </summary>
        [XmlElement("partner")]
        public string Partner { get; set; }

        /// <summary>
        /// 查询处理结果响应码:  SUCCESS：查询成功  FAIL：查询失败  PROCESS_EXCEPTION：处理异常
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
