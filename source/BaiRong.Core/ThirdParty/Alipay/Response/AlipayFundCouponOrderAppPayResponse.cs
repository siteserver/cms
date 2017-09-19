using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayFundCouponOrderAppPayResponse.
    /// </summary>
    public class AlipayFundCouponOrderAppPayResponse : AopResponse
    {
        /// <summary>
        /// 本次支付的金额，单位为：元（人民币），精确到小数点后两位
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 支付宝的资金授权订单号
        /// </summary>
        [XmlElement("auth_no")]
        public string AuthNo { get; set; }

        /// <summary>
        /// 资金授权成功时间  格式：YYYY-MM-DD HH:MM:SS
        /// </summary>
        [XmlElement("gmt_trans")]
        public string GmtTrans { get; set; }

        /// <summary>
        /// 支付宝的资金操作流水号
        /// </summary>
        [XmlElement("operation_id")]
        public string OperationId { get; set; }

        /// <summary>
        /// 商户的授权资金订单号
        /// </summary>
        [XmlElement("out_order_no")]
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 商户本次资金操作的请求流水号
        /// </summary>
        [XmlElement("out_request_no")]
        public string OutRequestNo { get; set; }

        /// <summary>
        /// 资金预授权明细的状态  目前支持：  INIT：初始  SUCCESS: 成功  CLOSED：关闭
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
