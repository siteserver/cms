using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoEduCampusJobCancelModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoEduCampusJobCancelModel : AopObject
    {
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
