using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCampaignRuleRulelistQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCampaignRuleRulelistQueryModel : AopObject
    {
        /// <summary>
        /// 签约商户下属子机构唯一编号
        /// </summary>
        [XmlElement("mpid")]
        public string Mpid { get; set; }

        /// <summary>
        /// 页码，从1开始
        /// </summary>
        [XmlElement("pageno")]
        public string Pageno { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        [XmlElement("pagesize")]
        public string Pagesize { get; set; }
    }
}
