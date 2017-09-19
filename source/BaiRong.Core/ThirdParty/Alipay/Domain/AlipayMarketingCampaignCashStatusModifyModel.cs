using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCampaignCashStatusModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCampaignCashStatusModifyModel : AopObject
    {
        /// <summary>
        /// 修改后的活动状态, PAUSE或者READY或者CLOSED
        /// </summary>
        [XmlElement("camp_status")]
        public string CampStatus { get; set; }

        /// <summary>
        /// 要修改的现金红包活动号
        /// </summary>
        [XmlElement("crowd_no")]
        public string CrowdNo { get; set; }
    }
}
