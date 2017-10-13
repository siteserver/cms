using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayPcreditLoanApplyCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayPcreditLoanApplyCreateModel : AopObject
    {
        /// <summary>
        /// 申贷金额，单位为元
        /// </summary>
        [XmlElement("apply_amt")]
        public string ApplyAmt { get; set; }

        /// <summary>
        /// 用户身份证后4位
        /// </summary>
        [XmlElement("cert_no")]
        public string CertNo { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 商户贴息率，0~1的小数
        /// </summary>
        [XmlElement("ratio")]
        public long Ratio { get; set; }

        /// <summary>
        /// 场景码
        /// </summary>
        [XmlElement("scene")]
        public string Scene { get; set; }
    }
}
