using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// EduAgeDemand Data Structure.
    /// </summary>
    [Serializable]
    public class EduAgeDemand : AopObject
    {
        /// <summary>
        /// 结束年龄
        /// </summary>
        [XmlElement("age_end")]
        public string AgeEnd { get; set; }

        /// <summary>
        /// 开始年龄
        /// </summary>
        [XmlElement("age_start")]
        public string AgeStart { get; set; }
    }
}
