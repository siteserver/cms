using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// OpenActivity Data Structure.
    /// </summary>
    [Serializable]
    public class OpenActivity : AopObject
    {
        /// <summary>
        /// 活动结束时间
        /// </summary>
        [XmlElement("activity_end_date")]
        public string ActivityEndDate { get; set; }

        /// <summary>
        /// 活动id
        /// </summary>
        [XmlElement("activity_id")]
        public string ActivityId { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        [XmlElement("activity_name")]
        public string ActivityName { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        [XmlElement("activity_start_date")]
        public string ActivityStartDate { get; set; }

        /// <summary>
        /// 活动验证状态  0，未检测  1，未通过  2，已通过
        /// </summary>
        [XmlElement("activity_status")]
        public string ActivityStatus { get; set; }

        /// <summary>
        /// 门店id
        /// </summary>
        [XmlElement("store_id")]
        public string StoreId { get; set; }
    }
}
