using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// QueryLabelRule Data Structure.
    /// </summary>
    [Serializable]
    public class QueryLabelRule : AopObject
    {
        /// <summary>
        /// 标签id
        /// </summary>
        [XmlElement("label_id")]
        public string LabelId { get; set; }

        /// <summary>
        /// 标签名
        /// </summary>
        [XmlElement("label_name")]
        public string LabelName { get; set; }

        /// <summary>
        /// 标签值，多值会用英文逗号分隔
        /// </summary>
        [XmlElement("label_value")]
        public string LabelValue { get; set; }

        /// <summary>
        /// 运算符
        /// </summary>
        [XmlElement("operator")]
        public string Operator { get; set; }
    }
}
