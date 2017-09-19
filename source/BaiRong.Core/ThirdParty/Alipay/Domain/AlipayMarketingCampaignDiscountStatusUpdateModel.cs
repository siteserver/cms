using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCampaignDiscountStatusUpdateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCampaignDiscountStatusUpdateModel : AopObject
    {
        /// <summary>
        /// 活动id
        /// </summary>
        [XmlElement("camp_id")]
        public string CampId { get; set; }

        /// <summary>
        /// 状态CAMP_PAUSED：暂停,CAMP_GOING 启动,CAMP_ENDED结束
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
