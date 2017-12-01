using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCampaignRuleCrowdQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCampaignRuleCrowdQueryModel : AopObject
    {
        /// <summary>
        /// 签约商户下属机构唯一编号
        /// </summary>
        [XmlElement("mpid")]
        public string Mpid { get; set; }

        /// <summary>
        /// 所要查询的规则id
        /// </summary>
        [XmlElement("ruleid")]
        public string Ruleid { get; set; }
    }
}
