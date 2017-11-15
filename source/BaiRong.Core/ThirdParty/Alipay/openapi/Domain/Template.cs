using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// Template Data Structure.
    /// </summary>
    [Serializable]
    public class Template : AopObject
    {
        /// <summary>
        /// 消息模板上下文，即模板中定义的参数及参数值
        /// </summary>
        [XmlElement("context")]
        public Context Context { get; set; }

        /// <summary>
        /// 消息模板ID
        /// </summary>
        [XmlElement("template_id")]
        public string TemplateId { get; set; }
    }
}
