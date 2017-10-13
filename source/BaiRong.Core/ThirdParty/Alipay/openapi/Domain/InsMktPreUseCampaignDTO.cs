using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsMktPreUseCampaignDTO Data Structure.
    /// </summary>
    [Serializable]
    public class InsMktPreUseCampaignDTO : AopObject
    {
        /// <summary>
        /// 活动Id
        /// </summary>
        [XmlElement("campaign_id")]
        public string CampaignId { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        [XmlElement("campaign_name")]
        public string CampaignName { get; set; }

        /// <summary>
        /// 权益类型
        /// </summary>
        [XmlElement("coupon_type")]
        public string CouponType { get; set; }

        /// <summary>
        /// 权益盖帽值
        /// </summary>
        [XmlElement("coupon_upper_value")]
        public string CouponUpperValue { get; set; }

        /// <summary>
        /// 权益值
        /// </summary>
        [XmlElement("coupon_value")]
        public string CouponValue { get; set; }

        /// <summary>
        /// 是否预核销通过
        /// </summary>
        [XmlElement("pre_use")]
        public bool PreUse { get; set; }

        /// <summary>
        /// 预核销失败原因
        /// </summary>
        [XmlElement("reason")]
        public string Reason { get; set; }
    }
}
