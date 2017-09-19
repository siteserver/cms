using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayFundAuthOperationCancelResponse.
    /// </summary>
    public class AlipayFundAuthOperationCancelResponse : AopResponse
    {
        /// <summary>
        /// 本次撤销触发的资金动作  close：关闭冻结明细，无资金解冻  unfreeze：产生了资金解冻
        /// </summary>
        [XmlElement("action")]
        public string Action { get; set; }

        /// <summary>
        /// 支付宝资金授权订单号。
        /// </summary>
        [XmlElement("auth_no")]
        public string AuthNo { get; set; }

        /// <summary>
        /// 支付宝的冻结操作流水号。
        /// </summary>
        [XmlElement("operation_id")]
        public string OperationId { get; set; }

        /// <summary>
        /// 商户的授权资金订单号。
        /// </summary>
        [XmlElement("out_order_no")]
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 商户的冻结操作流水号 。
        /// </summary>
        [XmlElement("out_request_no")]
        public string OutRequestNo { get; set; }
    }
}
