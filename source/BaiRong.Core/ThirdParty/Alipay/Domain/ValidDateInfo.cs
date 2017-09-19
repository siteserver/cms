using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ValidDateInfo Data Structure.
    /// </summary>
    [Serializable]
    public class ValidDateInfo : AopObject
    {
        /// <summary>
        /// 截至时间
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// 相对有效期
        /// </summary>
        [XmlElement("relative_time")]
        public PeriodInfo RelativeTime { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [XmlElement("start_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// 时间模式,RELATIVE=相对时间，RELATIVE=绝对模式
        /// </summary>
        [XmlElement("time_mode")]
        public string TimeMode { get; set; }
    }
}
