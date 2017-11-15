using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoEduCampusJobPublishModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoEduCampusJobPublishModel : AopObject
    {
        /// <summary>
        /// 过期时间(毫秒数)
        /// </summary>
        [XmlElement("gmt_expired")]
        public string GmtExpired { get; set; }

        /// <summary>
        /// 刷新时间(毫秒数)
        /// </summary>
        [XmlElement("gmt_refresh")]
        public string GmtRefresh { get; set; }

        /// <summary>
        /// 职位来源方编码
        /// </summary>
        [XmlElement("source_code")]
        public string SourceCode { get; set; }

        /// <summary>
        /// 职位在合作方的ID
        /// </summary>
        [XmlElement("source_id")]
        public string SourceId { get; set; }
    }
}
