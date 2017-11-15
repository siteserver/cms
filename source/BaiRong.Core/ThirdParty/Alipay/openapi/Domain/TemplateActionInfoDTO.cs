using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// TemplateActionInfoDTO Data Structure.
    /// </summary>
    [Serializable]
    public class TemplateActionInfoDTO : AopObject
    {
        /// <summary>
        /// 行动点业务CODE，商户自定义
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// 行动点展示文案
        /// </summary>
        [XmlElement("text")]
        public string Text { get; set; }

        /// <summary>
        /// 行动点跳转链接
        /// </summary>
        [XmlElement("url")]
        public string Url { get; set; }
    }
}
