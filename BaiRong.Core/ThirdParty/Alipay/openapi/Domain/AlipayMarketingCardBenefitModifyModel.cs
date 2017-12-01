using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCardBenefitModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCardBenefitModifyModel : AopObject
    {
        /// <summary>
        /// 权益ID
        /// </summary>
        [XmlElement("benefit_id")]
        public string BenefitId { get; set; }

        /// <summary>
        /// 会员卡模板外部权益
        /// </summary>
        [XmlElement("mcard_template_benefit")]
        public McardTemplateBenefit McardTemplateBenefit { get; set; }
    }
}
