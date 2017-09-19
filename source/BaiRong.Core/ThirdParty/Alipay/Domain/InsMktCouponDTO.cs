using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsMktCouponDTO Data Structure.
    /// </summary>
    [Serializable]
    public class InsMktCouponDTO : AopObject
    {
        /// <summary>
        /// 权益资产Id，如券Id
        /// </summary>
        [XmlElement("asset_id")]
        public string AssetId { get; set; }

        /// <summary>
        /// 权益Id
        /// </summary>
        [XmlElement("coupon_id")]
        public string CouponId { get; set; }

        /// <summary>
        /// 权益类型
        /// </summary>
        [XmlElement("coupon_type")]
        public string CouponType { get; set; }

        /// <summary>
        /// 500元单品券
        /// </summary>
        [XmlElement("coupon_value")]
        public string CouponValue { get; set; }

        /// <summary>
        /// 是否推荐使用该优惠
        /// </summary>
        [XmlElement("recommend")]
        public bool Recommend { get; set; }

        /// <summary>
        /// 核销结束时间
        /// </summary>
        [XmlElement("use_end_time")]
        public string UseEndTime { get; set; }

        /// <summary>
        /// 核销规则
        /// </summary>
        [XmlElement("use_rule")]
        public string UseRule { get; set; }

        /// <summary>
        /// 核销开始时间
        /// </summary>
        [XmlElement("use_start_time")]
        public string UseStartTime { get; set; }
    }
}
