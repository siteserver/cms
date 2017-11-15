using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ComplexLabelRule Data Structure.
    /// </summary>
    [Serializable]
    public class ComplexLabelRule : AopObject
    {
        /// <summary>
        /// 标签id
        /// </summary>
        [XmlElement("label_id")]
        public string LabelId { get; set; }

        /// <summary>
        /// 标签取值，当有多个取值时用英文","分隔（比如使用in操作符时）；不允许传入下划线"_"、竖线"|"或者空格" "
        /// </summary>
        [XmlElement("label_value")]
        public string LabelValue { get; set; }

        /// <summary>
        /// 目前支持EQ（等于）、NEQ（不等于）、LT（小于），GT（大于）、LTEQ（小于等于）、GTEQ（大于等于）、LIKE（匹配）、BETWEEN（范围）、IN（包含）、NOTIN（不包含）操作
        /// </summary>
        [XmlElement("operator")]
        public string Operator { get; set; }
    }
}
