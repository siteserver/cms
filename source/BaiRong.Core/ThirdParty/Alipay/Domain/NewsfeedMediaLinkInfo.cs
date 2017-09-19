using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// NewsfeedMediaLinkInfo Data Structure.
    /// </summary>
    [Serializable]
    public class NewsfeedMediaLinkInfo : AopObject
    {
        /// <summary>
        /// 资源ID
        /// </summary>
        [XmlElement("content_id")]
        public string ContentId { get; set; }

        /// <summary>
        /// 资源的来源
        /// </summary>
        [XmlElement("content_source")]
        public string ContentSource { get; set; }

        /// <summary>
        /// 资源类型
        /// </summary>
        [XmlElement("content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [XmlElement("desc")]
        public string Desc { get; set; }

        /// <summary>
        /// 链接的缩略图
        /// </summary>
        [XmlElement("thumb")]
        public string Thumb { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// linkurl
        /// </summary>
        [XmlElement("url")]
        public string Url { get; set; }
    }
}
