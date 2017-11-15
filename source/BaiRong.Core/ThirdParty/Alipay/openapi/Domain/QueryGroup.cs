using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// QueryGroup Data Structure.
    /// </summary>
    [Serializable]
    public class QueryGroup : AopObject
    {
        /// <summary>
        /// 分组id
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 分组中的圈人规则
        /// </summary>
        [XmlArray("label_rule")]
        [XmlArrayItem("query_complex_label_rule")]
        public List<QueryComplexLabelRule> LabelRule { get; set; }

        /// <summary>
        /// 用户分组名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}
