using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// PointCard Data Structure.
    /// </summary>
    [Serializable]
    public class PointCard : AopObject
    {
        /// <summary>
        /// 工具的描述
        /// </summary>
        [XmlElement("desc")]
        public string Desc { get; set; }

        /// <summary>
        /// 工具的有效期的结束时间（必须晚于活动的结束时间）
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// 工具的LOGO文件ID
        /// </summary>
        [XmlElement("logo")]
        public string Logo { get; set; }

        /// <summary>
        /// 工具的名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 工具的有效期的起始时间
        /// </summary>
        [XmlElement("start_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// 工具类型，目前支持：  集点卡：POINT_CARD
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}
