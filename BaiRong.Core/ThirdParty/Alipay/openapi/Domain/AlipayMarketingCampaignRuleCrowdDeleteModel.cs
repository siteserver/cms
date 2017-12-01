using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCampaignRuleCrowdDeleteModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCampaignRuleCrowdDeleteModel : AopObject
    {
        /// <summary>
        /// 签约商户下属子机构唯一编号
        /// </summary>
        [XmlElement("mpid")]
        public string Mpid { get; set; }

        /// <summary>
        /// 对没有在使用的规则进行删除
        /// </summary>
        [XmlElement("ruleid")]
        public string Ruleid { get; set; }
    }
}
