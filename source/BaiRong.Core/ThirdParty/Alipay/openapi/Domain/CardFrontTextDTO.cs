using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CardFrontTextDTO Data Structure.
    /// </summary>
    [Serializable]
    public class CardFrontTextDTO : AopObject
    {
        /// <summary>
        /// 文案标签
        /// </summary>
        [XmlElement("label")]
        public string Label { get; set; }

        /// <summary>
        /// 展示文案
        /// </summary>
        [XmlElement("value")]
        public string Value { get; set; }
    }
}
