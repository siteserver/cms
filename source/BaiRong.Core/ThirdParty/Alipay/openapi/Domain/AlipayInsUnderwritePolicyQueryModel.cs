using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsUnderwritePolicyQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsUnderwritePolicyQueryModel : AopObject
    {
        /// <summary>
        /// 商户生成的外部投保业务号;当保单凭证号不传时则必传.
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 保单凭证号;商户生成的外部投保业务号不传时则必传.
        /// </summary>
        [XmlElement("policy_no")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// 产品编码;当保单凭证号不传时则必传.
        /// </summary>
        [XmlElement("prod_code")]
        public string ProdCode { get; set; }
    }
}
