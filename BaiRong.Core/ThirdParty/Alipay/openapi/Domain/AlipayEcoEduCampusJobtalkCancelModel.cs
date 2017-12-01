using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoEduCampusJobtalkCancelModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoEduCampusJobtalkCancelModel : AopObject
    {
        /// <summary>
        /// 备用字段，json格式
        /// </summary>
        [XmlElement("content_var")]
        public string ContentVar { get; set; }

        /// <summary>
        /// 宣讲会来源方id
        /// </summary>
        [XmlElement("talk_source_code")]
        public string TalkSourceCode { get; set; }

        /// <summary>
        /// 宣讲会在合作方的ID
        /// </summary>
        [XmlElement("talk_source_id")]
        public string TalkSourceId { get; set; }
    }
}
