using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// QueryExtension Data Structure.
    /// </summary>
    [Serializable]
    public class QueryExtension : AopObject
    {
        /// <summary>
        /// 扩展区列表
        /// </summary>
        [XmlArray("areas")]
        [XmlArrayItem("extension_area")]
        public List<ExtensionArea> Areas { get; set; }

        /// <summary>
        /// 扩展区套id
        /// </summary>
        [XmlElement("extension_key")]
        public string ExtensionKey { get; set; }

        /// <summary>
        /// 标签规则列表
        /// </summary>
        [XmlArray("label_rules")]
        [XmlArrayItem("query_label_rule")]
        public List<QueryLabelRule> LabelRules { get; set; }

        /// <summary>
        /// 扩展区状态，"ON"代表上线，"OFF"代表下线，只有上线的扩展区才能被用户看到
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}
