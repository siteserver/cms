using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayFundCouponOrderRefundModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayFundCouponOrderRefundModel : AopObject
    {
        /// <summary>
        /// 需要退款的金额，单位为：元（人民币），精确到小数点后两位  取值范围：[0.01,100000000.00]
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 支付宝的资金授权订单号，即之前发红包时产生的支付宝订单号
        /// </summary>
        [XmlElement("auth_no")]
        public string AuthNo { get; set; }

        /// <summary>
        /// 商户本次资金操作的请求流水号  同一商户每次不同的资金操作请求，商户请求流水号不要重复
        /// </summary>
        [XmlElement("out_request_no")]
        public string OutRequestNo { get; set; }

        /// <summary>
        /// 商户对本次退款操作的附言描述，长度不超过100个字母或50个汉字
        /// </summary>
        [XmlElement("remark")]
        public string Remark { get; set; }
    }
}
