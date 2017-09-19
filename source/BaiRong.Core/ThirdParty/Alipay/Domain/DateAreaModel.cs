using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// DateAreaModel Data Structure.
    /// </summary>
    [Serializable]
    public class DateAreaModel : AopObject
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        [XmlElement("begin_date")]
        public string BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [XmlElement("end_date")]
        public string EndDate { get; set; }
    }
}
