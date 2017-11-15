using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// OrderLogisticsExtInfo Data Structure.
    /// </summary>
    [Serializable]
    public class OrderLogisticsExtInfo : AopObject
    {
        /// <summary>
        /// 服务结束时间，格式为yyyy-MM-dd HH:mm（到分）
        /// </summary>
        [XmlElement("gmt_end")]
        public string GmtEnd { get; set; }

        /// <summary>
        /// 服务开始时间，格式为yyyy-MM-dd HH:mm（到分）
        /// </summary>
        [XmlElement("gmt_start")]
        public string GmtStart { get; set; }
    }
}
