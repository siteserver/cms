using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// OpenPromoCamp Data Structure.
    /// </summary>
    [Serializable]
    public class OpenPromoCamp : AopObject
    {
        /// <summary>
        /// 简短活动名，默认和活动名称相同
        /// </summary>
        [XmlElement("camp_alias")]
        public string CampAlias { get; set; }

        /// <summary>
        /// 活动描述
        /// </summary>
        [XmlElement("camp_desc")]
        public string CampDesc { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        [XmlElement("camp_end_time")]
        public string CampEndTime { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        [XmlElement("camp_name")]
        public string CampName { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        [XmlElement("camp_start_time")]
        public string CampStartTime { get; set; }

        /// <summary>
        /// 活动类型，现在支持DRAW_PRIZE：表示领奖活动
        /// </summary>
        [XmlElement("camp_type")]
        public string CampType { get; set; }
    }
}
