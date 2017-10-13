using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayFundAuthOperationCancelModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayFundAuthOperationCancelModel : AopObject
    {
        /// <summary>
        /// 支付宝授权资金订单号，与商户的授权资金订单号不能同时为空，二者都存在时，以支付宝资金授权订单号为准，该参数与支付宝授权资金操作流水号配对使用。
        /// </summary>
        [XmlElement("auth_no")]
        public string AuthNo { get; set; }

        /// <summary>
        /// 支付宝的授权资金操作流水号，与商户的授权资金操作流水号不能同时为空，二者都存在时，以支付宝的授权资金操作流水号为准，该参数与支付宝授权资金订单号配对使用。
        /// </summary>
        [XmlElement("operation_id")]
        public string OperationId { get; set; }

        /// <summary>
        /// 商户的授权资金订单号，与支付宝的授权资金订单号不能同时为空，二者都存在时，以支付宝的授权资金订单号为准，该参数与商户的授权资金操作流水号配对使用。
        /// </summary>
        [XmlElement("out_order_no")]
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 商户的授权资金操作流水号，与支付宝的授权资金操作流水号不能同时为空，二者都存在时，以支付宝的授权资金操作流水号为准，该参数与商户的授权资金订单号配对使用。
        /// </summary>
        [XmlElement("out_request_no")]
        public string OutRequestNo { get; set; }

        /// <summary>
        /// 商户对本次撤销操作的附言描述，长度不超过100个字母或50个汉字
        /// </summary>
        [XmlElement("remark")]
        public string Remark { get; set; }
    }
}
