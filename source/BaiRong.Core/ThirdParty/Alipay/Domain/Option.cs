using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// Option Data Structure.
    /// </summary>
    [Serializable]
    public class Option : AopObject
    {
        /// <summary>
        /// 文本，通常用于理解对应的取值
        /// </summary>
        [XmlElement("text")]
        public string Text { get; set; }

        /// <summary>
        /// 取值，通常使用简单的数字或字符串
        /// </summary>
        [XmlElement("value")]
        public string Value { get; set; }
    }
}
