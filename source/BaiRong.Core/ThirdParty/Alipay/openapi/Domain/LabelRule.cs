using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// LabelRule Data Structure.
    /// </summary>
    [Serializable]
    public class LabelRule : AopObject
    {
        /// <summary>
        /// 标签id
        /// </summary>
        [XmlElement("label_id")]
        public string LabelId { get; set; }

        /// <summary>
        /// 标签值，当有多个取值时用英文","分隔，不允许传入下划线"_"、竖线"|"或者空格" "和方括号"["、"]"
        /// </summary>
        [XmlElement("label_value")]
        public string LabelValue { get; set; }

        /// <summary>
        /// 目前支持EQ（等于）、BETWEEN（范围）、IN（包含）三种操作符；每个标签支持的运算符可以通过查询接口获得。该字段允许为空，默认运算符为IN
        /// </summary>
        [XmlElement("operator")]
        public string Operator { get; set; }
    }
}
